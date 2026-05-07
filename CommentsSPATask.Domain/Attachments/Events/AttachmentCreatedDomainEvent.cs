using CommentsSPATask.Domain.Abstractions;

namespace CommentsSPATask.Domain.Attachments.Events;

public record AttachmentCreatedDomainEvent(Guid AttachmentId) : IDomainEvent;