using CommentsSPATask.Application.Abstractions.Messaging;

namespace CommentsSPATask.Application.Comments.Queries.GetCommentThread;

public sealed record GetCommentThreadQuery(Guid CommentId) : IQuery<CommentThreadResponse>;
