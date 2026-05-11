import type { Attachment, Comment } from "../../types/comments/commentType";
import type { GetCommentByIdResponse } from "../../types/comments/getCommentsResponseType";
import ThreadNode from "./ThreadNode";

type CommentThreadSectionProps = {
  error: unknown;
  isFetching: boolean;
  onAttachmentPreview: (attachment: Attachment) => void;
  onReply: (comment: Comment) => void;
  response?: GetCommentByIdResponse;
};

function CommentThreadSection({
  error,
  isFetching,
  onAttachmentPreview,
  onReply,
  response,
}: CommentThreadSectionProps) {
  return (
    <section
      className="inline-thread mt-3 pt-3"
      onClick={(event) => event.stopPropagation()}
    >
      {isFetching && <p className="alert alert-secondary mb-0">Loading replies...</p>}
      {Boolean(error) && (
        <p className="alert alert-danger mb-0">Could not load replies.</p>
      )}
      {response?.error && (
        <p className="alert alert-danger mb-0">
          {response.error.name.trim() || "Could not load replies."}
        </p>
      )}
      {response?.data?.replies?.length === 0 && (
        <p className="alert alert-secondary mb-0">No replies yet.</p>
      )}
      {response?.data?.replies?.map((reply) => (
        <ThreadNode
          comment={reply}
          key={reply.id}
          onAttachmentPreview={onAttachmentPreview}
          onReply={onReply}
        />
      ))}
    </section>
  );
}

export default CommentThreadSection;
