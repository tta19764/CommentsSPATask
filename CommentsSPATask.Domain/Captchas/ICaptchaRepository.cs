namespace CommentsSPATask.Domain.Captchas;

public interface ICaptchaRepository
{
    void Add(Captcha captcha);

    Task<Captcha?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<int> RemoveExpiredCaptchasAsync(DateTime cutoffUtc, CancellationToken cancellationToken = default);

    void Remove(Captcha captcha);
}
