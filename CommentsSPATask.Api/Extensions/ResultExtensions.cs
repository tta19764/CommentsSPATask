using CommentsSPATask.Api.Contracts;
using CommentsSPATask.Domain.Abstractions;

namespace CommentsSPATask.Api.Extensions;

public static class ResultExtensions
{
    public static ApiResponse<T> MapToApiResponse<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Metadata is not null
                ? new ApiResponse<T>
                {
                    Data = result.Value,
                    Metadata = result.Metadata.Value
                }
                : new ApiResponse<T>
                {
                    Data = result.Value
                };
        }

        return new ApiResponse<T>
        {
            Error = result.Error
        };
    }
}
