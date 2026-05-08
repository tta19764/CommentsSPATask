using CommentsSPATask.Domain.Attachments;

namespace CommentsSPATask.Application.Comments.Queries;

public sealed record CommentAttachmentResponse(
    Guid Id,
    string OriginalFileName,
    string StoredFileName,
    ContentType ContentType);
