namespace CommentsSPATask.Application.Captchas.Commands.CreateCaptcha;

public sealed record CreateCaptchaResponse(
    Guid CaptchaId,
    string ImageBase64,
    DateTime ExpiresAtUtc);