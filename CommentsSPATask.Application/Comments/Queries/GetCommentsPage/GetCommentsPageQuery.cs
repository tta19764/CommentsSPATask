using CommentsSPATask.Application.Abstractions.Messaging;

namespace CommentsSPATask.Application.Comments.Queries.GetCommentsPage;

public sealed record GetCommentsPageQuery(
    int Page = 1,
    int PageSize = 25,
    CommentSortField SortField = CommentSortField.CreatedAtUtc,
    SortDirection SortDirection = SortDirection.Desc) : IQuery<IReadOnlyCollection<CommentSummaryResponse>>;

public enum SortDirection
{
    Desc = 1,
    Asc = 2
}

public enum CommentSortField
{
    CreatedAtUtc = 1,
    UserName = 2,
    Email = 3
}
