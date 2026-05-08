using CommentsSPATask.Domain.Abstractions;

namespace CommentsSPATask.Domain.Captchas.Events;

public record CaptchaUsedDomainEvent(Guid CaptchaId) : IDomainEvent;
