using CommentsSPATask.Domain.Abstractions;

namespace CommentsSPATask.Domain.Captchas.Events;

public record CaptchaBlockedDomainEvent(Guid CaptchaId) : IDomainEvent;
