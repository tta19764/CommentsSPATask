using CommentsSPATask.Domain.Attachments;
using Microsoft.EntityFrameworkCore;

namespace CommentsSPATask.Infrastructure.Repositories;

internal sealed class AttachmentRepository(ApplicationDbContext context)
    : Repository<Attachment>(context), IAttachmentRepository
{
    public async Task<IReadOnlyCollection<Attachment>> GetByCommentIdAsync(
        Guid commentId,
        CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Attachment>()
            .Where(attachment => attachment.CommentId == commentId)
            .ToListAsync(cancellationToken);
    }
}
