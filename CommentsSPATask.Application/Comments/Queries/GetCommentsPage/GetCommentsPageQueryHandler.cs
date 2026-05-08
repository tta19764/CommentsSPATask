using CommentsSPATask.Application.Abstractions.Data;
using CommentsSPATask.Application.Abstractions.Messaging;
using CommentsSPATask.Application.Comments.Queries;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Attachments;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace CommentsSPATask.Application.Comments.Queries.GetCommentsPage;

public sealed class GetCommentsPageQueryHandler(
    IApplicationDbContext context,
    IAttachmentRepository attachmentRepository,
    ILogger<GetCommentsPageQueryHandler> logger)
    : IQueryHandler<GetCommentsPageQuery, IReadOnlyCollection<CommentSummaryResponse>>
{
    public async Task<Result<IReadOnlyCollection<CommentSummaryResponse>>> Handle(
        GetCommentsPageQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Loading comments page. Page: {Page}, PageSize: {PageSize}, SortField: {SortField}, SortDirection: {SortDirection}",
            request.Page,
            request.PageSize,
            request.SortField,
            request.SortDirection);

        var rootComments = context.Comments
            .AsNoTracking()
            .Where(comment => comment.ParentId == null);

        rootComments = ApplySorting(rootComments, request);

        var totalCount = await rootComments.CountAsync(cancellationToken);

        var comments = await rootComments
            .Skip(request.PageSize * (request.Page - 1))
            .Take(request.PageSize)
            .Select(comment => new CommentSummaryRow(
                comment.Id,
                comment.UserName.Value,
                comment.Email.Value,
                comment.HomePage != null ? comment.HomePage.Value : null,
                comment.Text.Value,
                comment.CreatedAtUtc))
            .ToListAsync(cancellationToken);
        
        var attachments = new Dictionary<Guid, IReadOnlyCollection<CommentAttachmentResponse>>(comments.Count);

        foreach (var comment in comments)
        {
            var commentAttachments = await attachmentRepository.GetByCommentIdAsync(comment.Id, cancellationToken);

            attachments[comment.Id] = commentAttachments
                .Select(attachment => new CommentAttachmentResponse(
                    attachment.Id,
                    attachment.OriginalFileName.Value,
                    attachment.StoredFileName.Value,
                    attachment.ContentType))
                .ToList();
        }

        var items = comments
            .Select(comment => new CommentSummaryResponse(
                comment.Id,
                comment.UserName,
                comment.Email,
                comment.HomePage,
                comment.Text,
                comment.CreatedAtUtc,
                attachments.TryGetValue(comment.Id, out var commentAttachments)
                    ? commentAttachments
                    : []))
            .ToList();

        logger.LogInformation(
            "Loaded comments page. Page: {Page}, ReturnedItems: {ReturnedItems}, TotalCount: {TotalCount}",
            request.Page,
            items.Count,
            totalCount);

        return Result.Success<IReadOnlyCollection<CommentSummaryResponse>>(items)
            .WithMetadata(new TotalCountMetadata(totalCount));
    }

    private static IQueryable<Domain.Comments.Comment> ApplySorting(
        IQueryable<Domain.Comments.Comment> comments,
        GetCommentsPageQuery request)
    {
        return request.SortDirection == SortDirection.Asc
            ? request.SortField switch
            {
                CommentSortField.CreatedAtUtc => comments.OrderBy(comment => comment.CreatedAtUtc),
                CommentSortField.UserName => comments.OrderBy(comment => comment.UserName.Value),
                CommentSortField.Email => comments.OrderBy(comment => comment.Email.Value),
                _ => comments.OrderBy(comment => comment.CreatedAtUtc)
            }
            : request.SortField switch
            {
                CommentSortField.CreatedAtUtc => comments.OrderByDescending(comment => comment.CreatedAtUtc),
                CommentSortField.UserName => comments.OrderByDescending(comment => comment.UserName.Value),
                CommentSortField.Email => comments.OrderByDescending(comment => comment.Email.Value),
                _ => comments.OrderByDescending(comment => comment.CreatedAtUtc)
            };
    }

    private sealed record CommentSummaryRow(
        Guid Id,
        string UserName,
        string Email,
        string? HomePage,
        string Text,
        DateTime CreatedAtUtc);
}
