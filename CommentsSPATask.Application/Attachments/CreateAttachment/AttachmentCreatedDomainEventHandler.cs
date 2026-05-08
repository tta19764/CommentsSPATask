using CommentsSPATask.Application.Abstractions.Data;
using CommentsSPATask.Application.Abstractions.Realtime;
using CommentsSPATask.Application.Comments.Realtime;
using CommentsSPATask.Domain.Attachments.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CommentsSPATask.Application.Attachments.CreateAttachment;

public sealed class AttachmentCreatedDomainEventHandler(
    IApplicationDbContext context,
    ICommentsRealtimeNotifier commentsRealtimeNotifier) : INotificationHandler<AttachmentCreatedDomainEvent>
{
    public async Task Handle(AttachmentCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var attachment = await context.Attachments
            .AsNoTracking()
            .Where(x => x.Id == notification.AttachmentId)
            .Select(x => new AttachmentCreatedRealtimeEvent(
                x.Id,
                x.CommentId,
                x.OriginalFileName.Value,
                x.StoredFileName.Value,
                x.ContentType,
                x.CreatedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);

        if (attachment is null)
        {
            return;
        }

        await commentsRealtimeNotifier.NotifyAttachmentCreatedAsync(attachment, cancellationToken);
    }
}
