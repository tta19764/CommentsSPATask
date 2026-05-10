import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { getApiBaseUrl, getApiPath } from "../utils/apiUrl";
import type { GetCommentsRequest } from "../types/comments/getCommentsRequestType";
import type {
  GetCommentsResponse,
  GetCommentByIdResponse,
} from "../types/comments/getCommentsResponseType";
import type { ApiResponse } from "../types/shared/apiResponse";
import type { Comment } from "../types/comments/commentType";

const COMMENTS_ENDPOINT = getApiPath(
  "VITE_APP_COMMENTS_ENDPOINT",
  import.meta.env.VITE_APP_COMMENTS_ENDPOINT
);

export const commentsApi = createApi({
    reducerPath: "commentsApi",

    baseQuery: fetchBaseQuery({
        baseUrl: getApiBaseUrl(),
    }),

    tagTypes: ["Comments"],

    endpoints: (builder) => ({
        getComments: builder.query<GetCommentsResponse, GetCommentsRequest>({
            query: ({ page, pageSize, sortField, sortDirection }) => ({
                url: COMMENTS_ENDPOINT,
                params: {
                Page: page,
                PageSize: pageSize,
                SortField: sortField,
                SortDirection: sortDirection,
                },
            }),

            providesTags: ["Comments"],
        }),

        getCommentById: builder.query<GetCommentByIdResponse, string>({
            query: (id) => ({
                url: `${COMMENTS_ENDPOINT}${id}`,
            }),

            providesTags: (_result, _error, id) => [{ type: "Comments", id }],
        }),

        createComment: builder.mutation<ApiResponse<Comment>, FormData>({
            query: (formData) => ({
                url: COMMENTS_ENDPOINT,
                method: "POST",
                body: formData,
            }),

            invalidatesTags: ["Comments"],
        }),

        createReply: builder.mutation<
            ApiResponse<Comment>,
            {
                parentId: string;
                formData: FormData;
            }
        >({
            query: ({ parentId, formData }) => ({
                url: `${COMMENTS_ENDPOINT}${parentId}/replies`,
                method: "POST",
                body: formData,
            }),

            invalidatesTags: (_result, _error, { parentId }) => [
                "Comments",
                { type: "Comments", id: parentId },
            ],
        }),
    }),
});

export const {
    useGetCommentsQuery,
    useGetCommentByIdQuery,
    useCreateCommentMutation,
    useCreateReplyMutation,
} = commentsApi;
