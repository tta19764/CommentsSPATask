using CommentsSPATask.Domain.Abstractions;

namespace CommentsSPATask.Application.Abstractions.Comments;

public interface ICommentHtmlPolicy
{
    Result<string> ValidateAndSanitize(string input);
}
