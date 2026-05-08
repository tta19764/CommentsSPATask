namespace CommentsSPATask.Domain.Comments;

public interface ICommentRepository
{
    void Add(Comment comment);

    Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
