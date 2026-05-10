using CommentsSPATask.Domain.Captchas;
using Microsoft.EntityFrameworkCore;

namespace CommentsSPATask.Infrastructure.Repositories;

internal sealed class CaptchaRepository(ApplicationDbContext context)
    : Repository<Captcha>(context), ICaptchaRepository
{
    public async Task<int> RemoveExpiredCaptchasAsync(
        DateTime cutoffUtc,
        CancellationToken cancellationToken = default)
    {
        var expiredCaptchas = await DbContext.Captchas
            .Where(captcha => captcha.ExpiresAtUtc <= cutoffUtc)
            .ToListAsync(cancellationToken);

        if (expiredCaptchas.Count == 0)
        {
            return 0;
        }

        DbContext.RemoveRange(expiredCaptchas);

        return expiredCaptchas.Count;
    }
}
