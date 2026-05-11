using CommentsSPATask.Application.Abstractions.Messaging;

namespace CommentsSPATask.Application.Comments.Queries.GetCommentsPage;

public sealed record GetCommentsPageQuery(
    int Page = 1,
    int PageSize = 25,
    CommentSortField SortField = CommentSortField.CreatedAtUtc,
    SortDirection SortDirection = SortDirection.Desc) : IQuery<IReadOnlyCollection<CommentSummaryResponse>>;

public enum SortDirection
{
    Asc = 1,
    Desc = 2
}

public enum CommentSortField
{
    UserName = 1,
    Email = 2,
    CreatedAtUtc = 3
}
