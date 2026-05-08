using CommentsSPATask.Domain.Captchas;

namespace CommentsSPATask.Infrastructure.Repositories;

internal sealed class CaptchaRepository(ApplicationDbContext context)
    : Repository<Captcha>(context), ICaptchaRepository
{
}
