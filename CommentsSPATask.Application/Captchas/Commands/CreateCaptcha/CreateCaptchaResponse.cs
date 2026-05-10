namespace CommentsSPATask.Application.Captchas.Commands.CreateCaptcha;

public sealed record CreateCaptchaResponse(
    Guid CaptchaId,
    DateTime ExpiresAtUtc);
