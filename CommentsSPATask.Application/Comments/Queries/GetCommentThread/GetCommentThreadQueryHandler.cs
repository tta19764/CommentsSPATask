using CommentsSPATask.Application.Abstractions.Data;
using CommentsSPATask.Application.Abstractions.Messaging;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Attachments;
using CommentsSPATask.Domain.Comments;
using Microsoft.EntityFrameworkCore;

namespace CommentsSPATask.Application.Comments.Queries.GetCommentThread;

public sealed class GetCommentThreadQueryHandler(
    IApplicationDbContext context,
    IAttachmentRepository attachmentRepository)
    : IQueryHandler<GetCommentThreadQuery, CommentThreadResponse>
{
    public async Task<Result<CommentThreadResponse>> Handle(
        GetCommentThreadQuery request,
        CancellationToken cancellationToken)
    {
        var rootComment = await context.Comments
            .AsNoTracking()
            .Where(comment => comment.Id == request.CommentId)
            .SingleOrDefaultAsync(cancellationToken);

        if (rootComment is null)
        {
            return Result.Failure<CommentThreadResponse>(CommentErrors.NotFound);
        }

        var allComments = await context.Comments
            .AsNoTracking()
            .Where(comment => comment.Id == request.CommentId || comment.ParentId != null)
            .ToListAsync(cancellationToken);

        var attachments = new Dictionary<Guid, IReadOnlyCollection<CommentAttachmentResponse>>(allComments.Count);

        foreach (var comment in allComments)
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

        var commentById = allComments.ToDictionary(comment => comment.Id);

        var lookup = allComments
            .ToLookup(comment => comment.ParentId);

        var thread = MapThread(rootComment.Id, commentById, lookup, attachments);

        return Result.Success(thread);
    }

    private static CommentThreadResponse MapThread(
        Guid commentId,
        IReadOnlyDictionary<Guid, Comment> commentById,
        ILookup<Guid?, Comment> lookup,
        IReadOnlyDictionary<Guid, IReadOnlyCollection<CommentAttachmentResponse>> attachments)
    {
        var comment = commentById[commentId];

        var replies = lookup[commentId]
            .OrderBy(child => child.CreatedAtUtc)
            .Select(child => MapThread(child.Id, commentById, lookup, attachments))
            .ToList();

        return new CommentThreadResponse(
            comment.Id,
            comment.ParentId,
            comment.UserName.Value,
            comment.Email.Value,
            comment.HomePage?.Value,
            comment.Text.Value,
            comment.CreatedAtUtc,
            attachments.TryGetValue(comment.Id, out var commentAttachments)
                ? commentAttachments
                : [],
            replies);
    }
}
