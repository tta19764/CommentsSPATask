namespace CommentsSPATask.Domain.Captchas;

public interface ICaptchaRepository
{
    Task AddAsync(Captcha captcha, CancellationToken cancellationToken = default);

    Task<Captcha?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task RemoveAsync(Captcha captcha, CancellationToken cancellationToken = default);
}
