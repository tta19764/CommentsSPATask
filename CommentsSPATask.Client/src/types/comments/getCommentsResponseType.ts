import type { ApiResponse } from "../shared/apiResponse";
import type { PaginationMetadata } from "../shared/listResponse";
import type { Comment } from "./commentType";

export type GetCommentsResponse = ApiResponse<Comment[], PaginationMetadata>;
export type GetCommentByIdResponse = ApiResponse<Comment, null>;
