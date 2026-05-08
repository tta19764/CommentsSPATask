using FluentValidation;

namespace CommentsSPATask.Application.Comments.Queries.GetCommentThread;

public sealed class GetCommentThreadQueryValidator : AbstractValidator<GetCommentThreadQuery>
{
    public GetCommentThreadQueryValidator()
    {
        RuleFor(query => query.CommentId)
            .NotEmpty();
    }
}
