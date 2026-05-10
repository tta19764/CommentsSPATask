import { getAttachmentUrl } from "../../api/attachmentsApi";
import type { Attachment } from "../../types/comments/commentType";

type AttachmentModalProps = {
  attachment: Attachment;
  onClose: () => void;
};

function AttachmentModal({ attachment, onClose }: AttachmentModalProps) {
  const attachmentUrl = getAttachmentUrl(attachment.storedFileName);

  return (
    <>
      <div
        aria-modal="true"
        className="modal fade show d-block"
        onClick={onClose}
        role="dialog"
        tabIndex={-1}
      >
        <div
          className="modal-dialog modal-dialog-centered modal-xl attachment-lightbox-dialog"
          onClick={(event) => event.stopPropagation()}
        >
          <div className="modal-content attachment-preview-content">
            <div className="modal-header">
              <h2 className="modal-title fs-6 text-break">
                {attachment.originalFileName}
              </h2>
              <button
                aria-label="Close attachment preview"
                className="btn-close"
                onClick={onClose}
                type="button"
              />
            </div>

            <div className="modal-body attachment-preview-body">
              {attachment.contentType === "Image" ? (
                <img alt={attachment.originalFileName} src={attachmentUrl} />
              ) : (
                <iframe src={attachmentUrl} title={attachment.originalFileName} />
              )}
            </div>
          </div>
        </div>
      </div>
      <div className="modal-backdrop fade show" />
    </>
  );
}

export default AttachmentModal;
