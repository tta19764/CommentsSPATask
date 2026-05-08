using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Attachments;

namespace CommentsSPATask.Application.Abstractions.Attachments;

public interface IAttachmentProcessor
{
    Task<Result<PreparedAttachment>> PrepareAsync(FileUpload upload, CancellationToken cancellationToken = default);
}

public sealed record PreparedAttachment(
    string OriginalFileName,
    string StoredFileName,
    ContentType ContentType);
