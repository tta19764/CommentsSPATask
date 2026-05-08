using CommentsSPATask.Application.Comments.Queries;

namespace CommentsSPATask.Application.Comments.Realtime;

public sealed record CommentCreatedRealtimeEvent(
    Guid Id,
    Guid? ParentId,
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    DateTime CreatedAtUtc,
    IReadOnlyCollection<CommentAttachmentResponse> Attachments);
