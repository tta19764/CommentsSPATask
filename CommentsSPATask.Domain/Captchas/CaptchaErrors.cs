using CommentsSPATask.Domain.Abstractions;

namespace CommentsSPATask.Domain.Captchas;

public static class CaptchaErrors
{
    public static readonly Error NotFound =
        new("Captcha.NotFound", "The captcha challenge was not found.");

    public static readonly Error Expired =
        new("Captcha.Expired", "The captcha challenge has expired.");

    public static readonly Error AlreadyUsed =
        new("Captcha.AlreadyUsed", "The captcha challenge has already been used.");

    public static readonly Error InvalidInput =
        new("Captcha.InvalidInput", "The captcha text is invalid.");
}
