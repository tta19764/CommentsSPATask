using CommentsSPATask.Domain.Comments;

namespace CommentsSPATask.Infrastructure.Repositories;

internal sealed class CommentRepository(ApplicationDbContext context) 
    : Repository<Comment>(context), ICommentRepository
{
}