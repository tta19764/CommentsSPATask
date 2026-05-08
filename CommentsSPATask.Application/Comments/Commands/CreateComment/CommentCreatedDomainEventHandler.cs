using CommentsSPATask.Application.Abstractions.Data;
using CommentsSPATask.Application.Abstractions.Realtime;
using CommentsSPATask.Application.Comments.Queries;
using CommentsSPATask.Application.Comments.Realtime;
using CommentsSPATask.Domain.Comments.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CommentsSPATask.Application.Comments.Commands.CreateComment;

public sealed class CommentCreatedDomainEventHandler(
    IApplicationDbContext context,
    ICommentsRealtimeNotifier commentsRealtimeNotifier) : INotificationHandler<CommentCreatedDomainEvent>
{
    public async Task Handle(CommentCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var comment = await context.Comments
            .AsNoTracking()
            .Where(x => x.Id == notification.CommentId)
            .Select(x => new
            {
                x.Id,
                x.ParentId,
                UserName = x.UserName.Value,
                Email = x.Email.Value,
                HomePage = x.HomePage != null ? x.HomePage.Value : null,
                Text = x.Text.Value,
                x.CreatedAtUtc
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (comment is null)
        {
            return;
        }

        var attachments = await context.Attachments
            .AsNoTracking()
            .Where(x => x.CommentId == comment.Id)
            .Select(x => new CommentAttachmentResponse(
                x.Id,
                x.OriginalFileName.Value,
                x.StoredFileName.Value,
                x.ContentType))
            .ToListAsync(cancellationToken);

        await commentsRealtimeNotifier.NotifyCommentCreatedAsync(
            new CommentCreatedRealtimeEvent(
                comment.Id,
                comment.ParentId,
                comment.UserName,
                comment.Email,
                comment.HomePage,
                comment.Text,
                comment.CreatedAtUtc,
                attachments),
            cancellationToken);
    }
}
