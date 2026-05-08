using CommentsSPATask.Domain.Abstractions;

namespace CommentsSPATask.Domain.Captchas.Events;

public record CaptchaCreatedDomainEvent(Guid CaptchaId) : IDomainEvent;