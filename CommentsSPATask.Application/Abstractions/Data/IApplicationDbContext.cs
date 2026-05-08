using CommentsSPATask.Domain.Attachments;
using CommentsSPATask.Domain.Comments;
using CommentsSPATask.Domain.Captchas;
using Microsoft.EntityFrameworkCore;

namespace CommentsSPATask.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<Comment> Comments { get; }
    
    DbSet<Attachment> Attachments { get; }
    
    DbSet<Domain.Captchas.Captcha> Captchas { get; }
}