import { useCommentFormModal } from "../../hooks/useCommentFormModal";
import CommentAttachmentInput from "./CommentAttachmentInput";
import CommentCaptchaFields from "./CommentCaptchaFields";
import CommentFormFooter from "./CommentFormFooter";
import CommentIdentityFields from "./CommentIdentityFields";
import CommentTextEditor from "./CommentTextEditor";

type CommentFormModalProps = {
  parentId?: string;
  title: string;
  onClose: () => void;
};

function CommentFormModal({ parentId, title, onClose }: CommentFormModalProps) {
  const form = useCommentFormModal({ onClose, parentId });

  return (
    <>
      <div
        aria-modal="true"
        className="modal fade show d-block"
        role="dialog"
        tabIndex={-1}
      >
        <div
          className="modal-dialog modal-dialog-centered modal-lg"
          onClick={(event) => event.stopPropagation()}
        >
          <form className="modal-content" onSubmit={form.handleSubmit}>
            <div className="modal-header">
              <h2 className="modal-title fs-5">{title}</h2>
              <button
                aria-label="Close"
                className="btn-close"
                onClick={onClose}
                type="button"
              />
            </div>

            <div className="modal-body">
              {form.submitError && (
                <p className="alert alert-danger">{form.submitError}</p>
              )}

              <div className="row g-3">
                <CommentIdentityFields
                  email={form.email}
                  homePage={form.homePage}
                  setEmail={form.setEmail}
                  setHomePage={form.setHomePage}
                  setUserName={form.setUserName}
                  userName={form.userName}
                />
                <CommentTextEditor setText={form.setText} text={form.text} />
                <CommentAttachmentInput
                  attachmentError={form.attachmentError}
                  onAttachmentChange={form.handleAttachmentChange}
                />
                <CommentCaptchaFields
                  captchaImageUrl={form.captchaImageUrl}
                  captchaInput={form.captchaInput}
                  isCaptchaLoading={form.isCaptchaLoading}
                  onRefresh={form.refreshCaptcha}
                  setCaptchaInput={form.setCaptchaInput}
                />
              </div>
            </div>

            <CommentFormFooter
              isSubmitDisabled={
                form.isSubmitting || Boolean(form.attachmentError)
              }
              isSubmitting={form.isSubmitting}
            />
          </form>
        </div>
      </div>
      <div className="modal-backdrop fade show" />
    </>
  );
}

export default CommentFormModal;
