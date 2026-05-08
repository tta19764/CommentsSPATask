using CommentsSPATask.Domain.Abstractions;

namespace CommentsSPATask.Domain.Captchas.Events;

public record CaptchaExpiredDomainEvent(Guid CaptchaId) : IDomainEvent;
