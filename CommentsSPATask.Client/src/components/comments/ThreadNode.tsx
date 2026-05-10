import { getAttachmentUrl } from "../../api/attachmentsApi";
import type { Attachment, Comment } from "../../types/comments/commentType";
import { formatDate } from "../../utils/formatDate";
import { sanitizeCommentHtml } from "../../utils/sanitizeCommentHtml";

type ThreadNodeProps = {
  comment: Comment;
  onAttachmentPreview: (attachment: Attachment) => void;
  onReply: (comment: Comment) => void;
};

function ThreadNode({ comment, onAttachmentPreview, onReply }: ThreadNodeProps) {
  const imageAttachments = comment.attachments.filter(
    (attachment) => attachment.contentType === "Image"
  );

  return (
    <div className="reply-branch">
      <article className="card bg-light-subtle">
        <div className="card-body">
          <header className="d-flex flex-column flex-md-row justify-content-between gap-2 mb-2">
            <strong>
              {comment.homePage ? (
                <a href={comment.homePage} rel="noreferrer" target="_blank">
                  {comment.userName}
                </a>
              ) : (
                comment.userName
              )}
            </strong>
            <time className="text-secondary small" dateTime={comment.createdAtUtc}>
              {formatDate(comment.createdAtUtc)}
            </time>
          </header>

          <div
            className="mb-3 comment-text"
            dangerouslySetInnerHTML={{ __html: sanitizeCommentHtml(comment.text) }}
          />

          {comment.attachments.length > 0 && (
            <div className="d-flex flex-wrap align-items-center gap-2 mb-3">
              {imageAttachments.slice(0, 2).map((attachment) => (
                <img
                  alt={attachment.originalFileName}
                  className="attachment-thumbnail"
                  key={attachment.id}
                  onClick={() => onAttachmentPreview(attachment)}
                  src={getAttachmentUrl(attachment.storedFileName)}
                />
              ))}

              {comment.attachments.map((attachment) => (
                <button
                  className="btn btn-sm btn-outline-primary"
                  key={attachment.id}
                  onClick={() => onAttachmentPreview(attachment)}
                  type="button"
                >
                  {attachment.originalFileName}
                </button>
              ))}
            </div>
          )}

          <div className="d-flex justify-content-end">
            <button
              className="btn btn-sm btn-outline-primary"
              onClick={() => onReply(comment)}
              type="button"
            >
              Add reply
            </button>
          </div>

          {comment.replies && comment.replies.length > 0 && (
            <div className="thread-replies mt-3">
              {comment.replies.map((reply) => (
                <ThreadNode
                  comment={reply}
                  key={reply.id}
                  onAttachmentPreview={onAttachmentPreview}
                  onReply={onReply}
                />
              ))}
            </div>
          )}
        </div>
      </article>
    </div>
  );
}

export default ThreadNode;
