export const CommentSortField = {
    UserName: 1,
    Email: 2,
    CreatedAtUtc: 3,
} as const;

export type CommentSortField = (typeof CommentSortField)[keyof typeof CommentSortField];

export const SortDirection = {
    Asc: 1,
    Desc: 2,
} as const;

export type SortDirection = (typeof SortDirection)[keyof typeof SortDirection];

export interface GetCommentsRequest {
    page: number;
    pageSize: number;
    sortField: CommentSortField;
    sortDirection: SortDirection;
}
