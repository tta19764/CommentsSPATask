namespace CommentsSPATask.Application.Abstractions.Captcha;

public interface ICaptchaImageStore
{
    Task StoreAsync(Guid captchaId, byte[] imageBytes, DateTime expiresAtUtc, CancellationToken cancellationToken = default);

    Task<byte[]?> GetAsync(Guid captchaId, CancellationToken cancellationToken = default);
}
