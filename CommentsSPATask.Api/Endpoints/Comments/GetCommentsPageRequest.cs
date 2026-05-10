using CommentsSPATask.Application.Comments.Queries.GetCommentsPage;

namespace CommentsSPATask.Api.Endpoints.Comments;

public sealed record GetCommentsPageRequest(
    int Page = 1,
    int PageSize = 25,
    CommentSortField SortField = CommentSortField.CreatedAtUtc,
    SortDirection SortDirection = SortDirection.Desc);
