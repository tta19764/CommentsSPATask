import type { MouseEvent } from "react";
import { getAttachmentUrl } from "../../api/attachmentsApi";
import type { Attachment } from "../../types/comments/commentType";

type CommentAttachmentsProps = {
  attachments: Attachment[];
  onPreview: (attachment: Attachment) => void;
  stopPropagation?: boolean;
};

function CommentAttachments({
  attachments,
  onPreview,
  stopPropagation = false,
}: CommentAttachmentsProps) {
  if (attachments.length === 0) {
    return null;
  }

  const imageAttachments = attachments.filter(
    (attachment) => attachment.contentType === "Image"
  );

  const handlePreview = (
    attachment: Attachment,
    event?: MouseEvent<HTMLElement>
  ) => {
    if (stopPropagation) {
      event?.stopPropagation();
    }

    onPreview(attachment);
  };

  return (
    <div className="d-flex flex-wrap align-items-center gap-2 mb-3">
      {imageAttachments.slice(0, 2).map((attachment) => (
        <img
          alt={attachment.originalFileName}
          className="attachment-thumbnail"
          key={attachment.id}
          onClick={(event) => handlePreview(attachment, event)}
          src={getAttachmentUrl(attachment.storedFileName)}
        />
      ))}

      {attachments.map((attachment) => (
        <button
          className="btn btn-sm btn-outline-primary"
          key={attachment.id}
          onClick={(event) => handlePreview(attachment, event)}
          type="button"
        >
          {attachment.originalFileName}
        </button>
      ))}
    </div>
  );
}

export default CommentAttachments;
