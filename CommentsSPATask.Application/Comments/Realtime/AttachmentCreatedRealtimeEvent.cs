using CommentsSPATask.Domain.Attachments;

namespace CommentsSPATask.Application.Comments.Realtime;

public sealed record AttachmentCreatedRealtimeEvent(
    Guid Id,
    Guid CommentId,
    string OriginalFileName,
    string StoredFileName,
    ContentType ContentType,
    DateTime CreatedAtUtc);
