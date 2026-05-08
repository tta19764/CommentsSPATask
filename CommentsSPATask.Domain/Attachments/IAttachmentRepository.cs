namespace CommentsSPATask.Domain.Attachments;

public interface IAttachmentRepository
{
    void Add(Attachment attachment);

    Task<Attachment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Attachment>> GetByCommentIdAsync(
        Guid commentId,
        CancellationToken cancellationToken = default);
}
