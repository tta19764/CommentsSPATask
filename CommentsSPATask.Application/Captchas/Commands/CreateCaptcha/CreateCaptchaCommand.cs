using CommentsSPATask.Application.Abstractions.Messaging;

namespace CommentsSPATask.Application.Captchas.Commands.CreateCaptcha;

public sealed record CreateCaptchaCommand : ICommand<CreateCaptchaResponse>;