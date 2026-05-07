namespace CommentsSPATask.Domain.Captcha;

public interface ICaptchaRepository
{
    Task AddAsync(Captcha captcha, CancellationToken cancellationToken = default);

    Task<Captcha?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
