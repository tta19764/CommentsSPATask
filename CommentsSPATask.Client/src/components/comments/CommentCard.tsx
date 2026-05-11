import { useGetCommentByIdQuery } from "../../api/commentsApi";
import type { Attachment, Comment } from "../../types/comments/commentType";
import CommentAttachments from "./CommentAttachments";
import CommentHeader from "./CommentHeader";
import CommentHtmlText from "./CommentHtmlText";
import CommentThreadSection from "./CommentThreadSection";

type CommentCardProps = {
  comment: Comment;
  isExpanded: boolean;
  onAttachmentPreview: (attachment: Attachment) => void;
  onReply: (comment: Comment) => void;
  onToggle: () => void;
};

function CommentCard({
  comment,
  isExpanded,
  onAttachmentPreview,
  onReply,
  onToggle,
}: CommentCardProps) {
  const {
    data: threadResponse,
    isFetching: isThreadFetching,
    error: threadError,
  } = useGetCommentByIdQuery(comment.id, {
    skip: !isExpanded,
  });

  return (
    <article
      aria-expanded={isExpanded}
      className="card comment-card"
      onClick={onToggle}
      role="button"
      tabIndex={0}
      onKeyDown={(event) => {
        if (event.key === "Enter" || event.key === " ") {
          event.preventDefault();
          onToggle();
        }
      }}
    >
      <div className="card-body">
        <CommentHeader comment={comment} stopPropagation />
        <CommentHtmlText text={comment.text} />
        <CommentAttachments
          attachments={comment.attachments}
          onPreview={onAttachmentPreview}
          stopPropagation
        />

        <div className="d-flex justify-content-end">
          <div className="btn-group">
            <button
              className="btn btn-sm btn-outline-primary"
              onClick={(event) => {
                event.stopPropagation();
                onReply(comment);
              }}
              type="button"
            >
              Add reply
            </button>
            <button className="btn btn-sm btn-primary" type="button">
              {isExpanded ? "Hide replies" : "Show replies"}
            </button>
          </div>
        </div>

        {isExpanded && (
          <CommentThreadSection
            error={threadError}
            isFetching={isThreadFetching}
            onAttachmentPreview={onAttachmentPreview}
            onReply={onReply}
            response={threadResponse}
          />
        )}
      </div>
    </article>
  );
}

export default CommentCard;
