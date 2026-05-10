using CommentsSPATask.Api.Endpoints.Captchas;
using CommentsSPATask.Api.Endpoints.Comments;
using Microsoft.AspNetCore.Routing;

namespace CommentsSPATask.Api.Extensions;

public static class EndpointMappings
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapCommentEndpoints();
        builder.MapCaptchaEndpoints();

        return builder;
    }
}
