namespace CommentsSPATask.Domain.Comments;

public interface ICommentRepository
{
    Task AddAsync(Comment comment, CancellationToken cancellationToken = default);

    Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
