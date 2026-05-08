using CommentsSPATask.Domain.Abstractions;
using MediatR;

namespace CommentsSPATask.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}