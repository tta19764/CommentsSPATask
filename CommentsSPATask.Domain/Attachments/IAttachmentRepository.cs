namespace CommentsSPATask.Domain.Attachments;

public interface IAttachmentRepository
{
    Task AddAsync(Attachment attachment, CancellationToken cancellationToken = default);

    Task<Attachment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Attachment>> GetByCommentIdAsync(
        Guid commentId,
        CancellationToken cancellationToken = default);
}
