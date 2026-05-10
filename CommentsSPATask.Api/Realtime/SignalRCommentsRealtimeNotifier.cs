using CommentsSPATask.Api.Hubs;
using CommentsSPATask.Application.Abstractions.Realtime;
using CommentsSPATask.Application.Comments.Realtime;
using Microsoft.AspNetCore.SignalR;

namespace CommentsSPATask.Api.Realtime;

public sealed class SignalRCommentsRealtimeNotifier(
    IHubContext<CommentsHub, ICommentsHubClient> hubContext) : ICommentsRealtimeNotifier
{
    public Task NotifyCommentCreatedAsync(
        CommentCreatedRealtimeEvent comment,
        CancellationToken cancellationToken = default)
    {
        return hubContext.Clients.All.CommentCreated(comment);
    }

    public Task NotifyAttachmentCreatedAsync(
        AttachmentCreatedRealtimeEvent attachment,
        CancellationToken cancellationToken = default)
    {
        return hubContext.Clients.All.AttachmentCreated(attachment);
    }
}
