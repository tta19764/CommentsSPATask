using System;

namespace CommentsSPATask.Api.Endpoints.Captchas;

public sealed record CreateCaptchaApiResponse(
    Guid CaptchaId,
    string ImageUrl,
    DateTime ExpiresAtUtc);
