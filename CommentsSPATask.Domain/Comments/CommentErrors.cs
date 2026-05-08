using CommentsSPATask.Domain.Abstractions;

namespace CommentsSPATask.Domain.Comments;

public static class CommentErrors
{
    public static readonly Error NotFound =
        new("Comments.NotFound", "The comment was not found.");

    public static readonly Error ParentCommentNotFound =
        new("Comments.ParentNotFound", "The parent comment was not found.");

    public static readonly Error InvalidHtml =
        new("Comments.InvalidHtml", "The comment text contains invalid or forbidden HTML.");
}
