using CommentsSPATask.Application.Comments.Realtime;

namespace CommentsSPATask.Application.Abstractions.Realtime;

public interface ICommentsRealtimeNotifier
{
    Task NotifyCommentCreatedAsync(CommentCreatedRealtimeEvent comment, CancellationToken cancellationToken = default);

    Task NotifyAttachmentCreatedAsync(AttachmentCreatedRealtimeEvent attachment, CancellationToken cancellationToken = default);
}
