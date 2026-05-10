using Microsoft.AspNetCore.Mvc;

namespace CommentsSPATask.Api.Endpoints.Comments;

public sealed class CreateCommentRequest
{
    [FromForm]
    public string UserName { get; init; } = string.Empty;

    [FromForm]
    public string Email { get; init; } = string.Empty;

    [FromForm]
    public string? HomePage { get; init; }

    [FromForm]
    public string Text { get; init; } = string.Empty;

    [FromForm]
    public Guid CaptchaId { get; init; }

    [FromForm]
    public string CaptchaInput { get; init; } = string.Empty;

    [FromForm]
    public Guid? ParentId { get; init; }

    [FromForm]
    public IFormFile? Attachment { get; init; }
}
