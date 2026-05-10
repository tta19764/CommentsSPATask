import { useGetCommentByIdQuery } from "../../api/commentsApi";
import { getAttachmentUrl } from "../../api/attachmentsApi";
import type { Attachment, Comment } from "../../types/comments/commentType";
import { formatDate } from "../../utils/formatDate";
import { sanitizeCommentHtml } from "../../utils/sanitizeCommentHtml";
import ThreadNode from "./ThreadNode";

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

  const imageAttachments = comment.attachments.filter(
    (attachment) => attachment.contentType === "Image"
  );

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
        <header className="d-flex flex-column flex-md-row justify-content-between gap-2 mb-3">
          <div>
            <h2 className="h5 mb-1">
              {comment.homePage ? (
                <a
                  href={comment.homePage}
                  onClick={(event) => event.stopPropagation()}
                  rel="noreferrer"
                  target="_blank"
                >
                  {comment.userName}
                </a>
              ) : (
                comment.userName
              )}
            </h2>
            <a
              href={`mailto:${comment.email}`}
              onClick={(event) => event.stopPropagation()}
            >
              {comment.email}
            </a>
          </div>
          <time className="text-secondary small" dateTime={comment.createdAtUtc}>
            {formatDate(comment.createdAtUtc)}
          </time>
        </header>

        <div
          className="comment-text text-secondary mb-3"
          dangerouslySetInnerHTML={{ __html: sanitizeCommentHtml(comment.text) }}
        />

        {comment.attachments.length > 0 && (
          <div className="d-flex flex-wrap align-items-center gap-2 mb-3">
            {imageAttachments.slice(0, 2).map((attachment) => (
              <img
                alt={attachment.originalFileName}
                className="attachment-thumbnail"
                key={attachment.id}
                onClick={(event) => {
                  event.stopPropagation();
                  onAttachmentPreview(attachment);
                }}
                src={getAttachmentUrl(attachment.storedFileName)}
              />
            ))}

            {comment.attachments.map((attachment) => (
              <button
                className="btn btn-sm btn-outline-primary"
                key={attachment.id}
                onClick={(event) => {
                  event.stopPropagation();
                  onAttachmentPreview(attachment);
                }}
                type="button"
              >
                {attachment.originalFileName}
              </button>
            ))}
          </div>
        )}

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
          <section
            className="inline-thread mt-3 pt-3"
            onClick={(event) => event.stopPropagation()}
          >
            {isThreadFetching && (
              <p className="alert alert-secondary mb-0">Loading replies...</p>
            )}
            {threadError && (
              <p className="alert alert-danger mb-0">Could not load replies.</p>
            )}
            {threadResponse?.error && (
              <p className="alert alert-danger mb-0">
                {threadResponse.error.name.trim() || "Could not load replies."}
              </p>
            )}
            {threadResponse?.data?.replies?.length === 0 && (
              <p className="alert alert-secondary mb-0">No replies yet.</p>
            )}
            {threadResponse?.data?.replies?.map((reply) => (
              <ThreadNode
                comment={reply}
                key={reply.id}
                onAttachmentPreview={onAttachmentPreview}
                onReply={onReply}
              />
            ))}
          </section>
        )}
      </div>
    </article>
  );
}

export default CommentCard;
