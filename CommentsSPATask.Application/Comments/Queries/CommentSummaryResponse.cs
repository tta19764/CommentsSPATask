namespace CommentsSPATask.Application.Comments.Queries;

public sealed record CommentSummaryResponse(
    Guid Id,
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    DateTime CreatedAtUtc,
    IReadOnlyCollection<CommentAttachmentResponse> Attachments);
