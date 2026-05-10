import { useState } from "react";
import { useSearchParams } from "react-router-dom";
import type { Attachment, Comment } from "../types/comments/commentType";
import AttachmentModal from "../components/comments/AttachmentModal";
import CommentCard from "../components/comments/CommentCard";
import CommentsPagination from "../components/comments/CommentsPagination";
import CommentFormModal from "../components/comments/CommentFormModal";
import PageHeader from "../components/comments/PageHeader";
import SortToolbar from "../components/comments/SortToolbar";
import { useGetCommentsQuery } from "../api/commentsApi";
import {
  CommentSortField,
  SortDirection,
  type CommentSortField as CommentSortFieldValue,
  type SortDirection as SortDirectionValue,
} from "../types/comments/getCommentsRequestType";

const PAGE_SIZE = 25;

function readNumberParam(
  searchParams: URLSearchParams,
  key: string,
  fallback: number,
  allowedValues?: readonly number[]
) {
  const value = Number(searchParams.get(key));

  if (!Number.isInteger(value) || value <= 0) {
    return fallback;
  }

  return allowedValues && !allowedValues.includes(value) ? fallback : value;
}

function CommentsPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [expandedCommentId, setExpandedCommentId] = useState<string | null>(null);
  const [previewAttachment, setPreviewAttachment] = useState<Attachment | null>(null);
  const [isAddCommentOpen, setIsAddCommentOpen] = useState(false);
  const [replyTarget, setReplyTarget] = useState<Comment | null>(null);

  const page = readNumberParam(searchParams, "page", 1);
  const sortField = readNumberParam(
    searchParams,
    "sortField",
    CommentSortField.CreatedAtUtc,
    Object.values(CommentSortField)
  ) as CommentSortFieldValue;
  const sortDirection = readNumberParam(
    searchParams,
    "sortDirection",
    SortDirection.Desc,
    Object.values(SortDirection)
  ) as SortDirectionValue;

  const { data: response, isFetching, isLoading, error } = useGetCommentsQuery({
    page,
    pageSize: PAGE_SIZE,
    sortField,
    sortDirection,
  });

  const comments = response?.data ?? [];
  const totalCount = response?.metadata?.totalCount ?? comments.length;
  const totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));

  const updateSearch = (nextValues: Record<string, number>) => {
    const nextParams = new URLSearchParams(searchParams);

    Object.entries(nextValues).forEach(([key, value]) => {
      nextParams.set(key, String(value));
    });

    setSearchParams(nextParams);
  };

  const changeSort = (nextSortField: CommentSortFieldValue) => {
    const nextDirection =
      sortField === nextSortField && sortDirection === SortDirection.Desc
        ? SortDirection.Asc
        : SortDirection.Desc;

    updateSearch({
      page: 1,
      sortField: nextSortField,
      sortDirection: nextDirection,
    });
  };

  return (
    <main className="container py-4 py-md-5">
      <PageHeader
        onAddComment={() => setIsAddCommentOpen(true)}
        totalCount={totalCount}
      />

      <SortToolbar
        onSortChange={changeSort}
        sortDirection={sortDirection}
        sortField={sortField}
      />

      {isLoading && <p className="alert alert-secondary">Loading comments...</p>}

      {error && (
        <p className="alert alert-danger">
          Could not load comments. Check that the API is running.
        </p>
      )}

      {!isLoading && !error && comments.length === 0 && (
        <p className="alert alert-secondary">No comments yet.</p>
      )}

      <section className="d-grid gap-3" aria-busy={isFetching}>
        {comments.map((comment) => (
          <CommentCard
            comment={comment}
            isExpanded={expandedCommentId === comment.id}
            key={comment.id}
            onAttachmentPreview={setPreviewAttachment}
            onReply={setReplyTarget}
            onToggle={() =>
              setExpandedCommentId((currentId) =>
                currentId === comment.id ? null : comment.id
              )
            }
          />
        ))}
      </section>

      <CommentsPagination
        currentPage={page}
        totalPages={totalPages}
        onPageChange={(nextPage) => updateSearch({ page: nextPage })}
      />

      {previewAttachment && (
        <AttachmentModal
          attachment={previewAttachment}
          onClose={() => setPreviewAttachment(null)}
        />
      )}

      {isAddCommentOpen && (
        <CommentFormModal
          onClose={() => setIsAddCommentOpen(false)}
          title="Add comment"
        />
      )}

      {replyTarget && (
        <CommentFormModal
          onClose={() => setReplyTarget(null)}
          parentId={replyTarget.id}
          title={`Reply to ${replyTarget.userName}`}
        />
      )}
    </main>
  );
}

export default CommentsPage;
