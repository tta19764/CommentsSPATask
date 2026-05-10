using System.Threading.Tasks;
using CommentsSPATask.Application.Comments.Realtime;

namespace CommentsSPATask.Api.Hubs;

public interface ICommentsHubClient
{
    Task CommentCreated(CommentCreatedRealtimeEvent comment);

    Task AttachmentCreated(AttachmentCreatedRealtimeEvent attachment);
}
