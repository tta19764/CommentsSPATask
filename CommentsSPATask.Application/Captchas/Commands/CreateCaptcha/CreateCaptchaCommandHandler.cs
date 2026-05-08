using CommentsSPATask.Application.Abstractions.Captcha;
using CommentsSPATask.Application.Abstractions.Clock;
using CommentsSPATask.Application.Abstractions.Messaging;
using CommentsSPATask.Domain.Abstractions;
using CommentsSPATask.Domain.Captchas;
using Microsoft.Extensions.Logging;

namespace CommentsSPATask.Application.Captchas.Commands.CreateCaptcha;

public sealed class CreateCaptchaCommandHandler(
    ICaptchaRepository captchaRepository,
    ICaptchaChallengeService captchaChallengeService,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork,
    ILogger<CreateCaptchaCommandHandler> logger)
    : ICommandHandler<CreateCaptchaCommand, CreateCaptchaResponse>
{
    public async Task<Result<CreateCaptchaResponse>> Handle(CreateCaptchaCommand request, CancellationToken cancellationToken)
    {
        var createdAtUtc = dateTimeProvider.UtcNow;
        var expiresAtUtc = createdAtUtc.AddMinutes(5);
        logger.LogInformation("Creating captcha challenge. ExpiresAtUtc: {ExpiresAtUtc}", expiresAtUtc);

        var code = captchaChallengeService.GenerateCode(5);
        var hash = captchaChallengeService.HashCode(code);

        var captcha = Captcha.Create(
            new CodeHash(hash),
            createdAtUtc,
            expiresAtUtc);

        await captchaRepository.AddAsync(captcha, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Captcha {CaptchaId} was persisted successfully", captcha.Id);

        var imageBase64 = Convert.ToBase64String(captchaChallengeService.RenderImage(code));
        logger.LogInformation("Captcha {CaptchaId} image was rendered", captcha.Id);

        return Result.Success(new CreateCaptchaResponse(captcha.Id, imageBase64, expiresAtUtc));
    }
}
