using CommentsSPATask.Domain.Abstractions;

namespace CommentsSPATask.Domain.Captcha.Events;

public record CaptchaCreatedDomainEvent(Guid CaptchaId) : IDomainEvent;