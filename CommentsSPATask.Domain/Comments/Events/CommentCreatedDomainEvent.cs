using CommentsSPATask.Domain.Abstractions;

namespace CommentsSPATask.Domain.Comments.Events;

public record CommentCreatedDomainEvent(Guid CommentId) : IDomainEvent;