using FluentValidation;

namespace CommentsSPATask.Application.Comments.Queries.GetCommentsPage;

public sealed class GetCommentsPageQueryValidator : AbstractValidator<GetCommentsPageQuery>
{
    public GetCommentsPageQueryValidator()
    {
        RuleFor(query => query.Page)
            .GreaterThan(0);

        RuleFor(query => query.PageSize)
            .GreaterThan(0);

        RuleFor(query => query.SortField)
            .IsInEnum();

        RuleFor(query => query.SortDirection)
            .IsInEnum();
    }
}
