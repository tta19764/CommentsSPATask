using CommentsSPATask.Api.Endpoints.Captchas;
using CommentsSPATask.Api.Endpoints.Comments;
using CommentsSPATask.Api.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace CommentsSPATask.Api.Extensions;

public static class EndpointMappings
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapCommentEndpoints();
        builder.MapCaptchaEndpoints();
        builder.MapHub<CommentsHub>("/hubs/comments")
            .RequireCors("Client");

        return builder;
    }
}
