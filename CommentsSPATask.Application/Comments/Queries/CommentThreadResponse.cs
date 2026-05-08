namespace CommentsSPATask.Application.Comments.Queries;

public sealed record CommentThreadResponse(
    Guid Id,
    Guid? ParentId,
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    DateTime CreatedAtUtc,
    IReadOnlyCollection<CommentAttachmentResponse> Attachments,
    IReadOnlyCollection<CommentThreadResponse> Replies);
