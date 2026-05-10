using Microsoft.AspNetCore.SignalR;

namespace CommentsSPATask.Api.Hubs;

public sealed class CommentsHub : Hub<ICommentsHubClient>
{
}
