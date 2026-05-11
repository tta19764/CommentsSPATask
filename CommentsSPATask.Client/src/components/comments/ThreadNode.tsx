import type { Attachment, Comment } from "../../types/comments/commentType";
import CommentAttachments from "./CommentAttachments";
import CommentHeader from "./CommentHeader";
import CommentHtmlText from "./CommentHtmlText";

type ThreadNodeProps = {
  comment: Comment;
  onAttachmentPreview: (attachment: Attachment) => void;
  onReply: (comment: Comment) => void;
};

function ThreadNode({ comment, onAttachmentPreview, onReply }: ThreadNodeProps) {
  return (
    <div className="reply-branch">
      <article className="card bg-light-subtle">
        <div className="card-body">
          <CommentHeader comment={comment} titleClassName="fw-semibold" />
          <CommentHtmlText className="mb-3 comment-text" text={comment.text} />
          <CommentAttachments
            attachments={comment.attachments}
            onPreview={onAttachmentPreview}
          />

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
