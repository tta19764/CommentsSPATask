using System.Collections.Generic;
using CommentsSPATask.Domain.Abstractions;

namespace CommentsSPATask.Api.Contracts;

public record ApiResponse<T>
{
    public T? Data { get; init; }

    public Dictionary<string, object>? Metadata { get; init; }

    public Error? Error { get; init; }
}
