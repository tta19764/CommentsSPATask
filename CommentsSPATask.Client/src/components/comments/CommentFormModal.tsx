import { type FormEvent, useEffect, useState } from "react";
import { useCreateCaptchaMutation } from "../../api/captchasApi";
import {
  useCreateCommentMutation,
  useCreateReplyMutation,
} from "../../api/commentsApi";
import { buildCommentFormData } from "../../builders/buildCommentFormData";
import type { ApiResponse } from "../../types/shared/apiResponse";
import { buildApiUrl } from "../../utils/apiUrl";

type CommentFormModalProps = {
  parentId?: string;
  title: string;
  onClose: () => void;
};

function CommentFormModal({ parentId, title, onClose }: CommentFormModalProps) {
  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [homePage, setHomePage] = useState("");
  const [text, setText] = useState("");
  const [captchaInput, setCaptchaInput] = useState("");
  const [attachment, setAttachment] = useState<File | undefined>();
  const [submitError, setSubmitError] = useState<string | null>(null);

  const [createCaptcha, { data: captchaResponse, isLoading: isCaptchaLoading }] =
    useCreateCaptchaMutation();
  const [createComment, { isLoading: isCreatingComment }] =
    useCreateCommentMutation();
  const [createReply, { isLoading: isCreatingReply }] = useCreateReplyMutation();

  const captcha = captchaResponse?.data;
  const isSubmitting = isCreatingComment || isCreatingReply;
  const allowedTags = [
    { label: "a", value: '<a href="" title=""></a>' },
    { label: "code", value: "<code></code>" },
    { label: "i", value: "<i></i>" },
    { label: "strong", value: "<strong></strong>" },
  ];

  useEffect(() => {
    void createCaptcha();
  }, [createCaptcha]);

  const captchaImageUrl = captcha?.imageUrl
    ? buildApiUrl(captcha.imageUrl)
    : null;

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setSubmitError(null);

    if (!captcha?.captchaId) {
      setSubmitError("Captcha is not ready yet.");
      return;
    }

    const formData = buildCommentFormData({
      userName,
      email,
      homePage,
      text,
      captchaId: captcha.captchaId,
      captchaInput,
      attachment,
    });

    const result = parentId
      ? await createReply({ parentId, formData })
      : await createComment(formData);

    if ("error" in result) {
      const apiError = getApiErrorFromMutationError(result.error);
      setSubmitError(
        getApiErrorText(apiError) ??
          "Could not save the comment. Check the fields and captcha."
      );

      void createCaptcha();
      setCaptchaInput("");
      return;
    }

    if (result.data?.error) {
      setSubmitError(
        getApiErrorText(result.data.error) ?? "Could not save the comment."
      );

      void createCaptcha();
      setCaptchaInput("");
      return;
    }

    onClose();
  };

  const insertTag = (tagValue: string) => {
    setText((currentText) => `${currentText}${tagValue}`);
  };

  return (
    <>
      <div
        aria-modal="true"
        className="modal fade show d-block"
        onClick={(event) => {
          event.stopPropagation();
          onClose();
        }}
        role="dialog"
        tabIndex={-1}
      >
        <div
          className="modal-dialog modal-dialog-centered modal-lg"
          onClick={(event) => event.stopPropagation()}
        >
          <form className="modal-content" onSubmit={handleSubmit}>
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
              {submitError && <p className="alert alert-danger">{submitError}</p>}

              <div className="row g-3">
                <div className="col-md-6">
                  <label className="form-label" htmlFor="comment-user-name">
                    User name
                  </label>
                  <input
                    className="form-control"
                    id="comment-user-name"
                    onChange={(event) => setUserName(event.target.value)}
                    pattern="[A-Za-z0-9]+"
                    required
                    value={userName}
                  />
                </div>

                <div className="col-md-6">
                  <label className="form-label" htmlFor="comment-email">
                    E-mail
                  </label>
                  <input
                    className="form-control"
                    id="comment-email"
                    onChange={(event) => setEmail(event.target.value)}
                    required
                    type="email"
                    value={email}
                  />
                </div>

                <div className="col-12">
                  <label className="form-label" htmlFor="comment-home-page">
                    Home page
                  </label>
                  <input
                    className="form-control"
                    id="comment-home-page"
                    onChange={(event) => setHomePage(event.target.value)}
                    type="url"
                    value={homePage}
                  />
                </div>

                <div className="col-12">
                  <div className="d-flex flex-column flex-md-row justify-content-between gap-2 align-items-md-end">
                    <label className="form-label" htmlFor="comment-text">
                      Text
                    </label>
                    <div aria-label="Allowed HTML tags" className="btn-group btn-group-sm">
                      {allowedTags.map((tag) => (
                        <button
                          className="btn btn-outline-secondary"
                          key={tag.label}
                          onClick={() => insertTag(tag.value)}
                          type="button"
                        >
                          {tag.label}
                        </button>
                      ))}
                    </div>
                  </div>
                  <textarea
                    className="form-control"
                    id="comment-text"
                    onChange={(event) => setText(event.target.value)}
                    required
                    rows={5}
                    value={text}
                  />
                </div>

                <div className="col-12">
                  <label className="form-label" htmlFor="comment-attachment">
                    Attachment
                  </label>
                  <input
                    accept=".jpg,.jpeg,.png,.gif,.txt,image/jpeg,image/png,image/gif,text/plain"
                    className="form-control"
                    id="comment-attachment"
                    onChange={(event) =>
                      setAttachment(event.target.files?.[0] ?? undefined)
                    }
                    type="file"
                  />
                </div>

                <div className="col-md-6">
                  <label className="form-label" htmlFor="comment-captcha">
                    CAPTCHA
                  </label>
                  <input
                    className="form-control"
                    id="comment-captcha"
                    onChange={(event) => setCaptchaInput(event.target.value)}
                    required
                    value={captchaInput}
                  />
                </div>

                <div className="col-md-6 d-flex align-items-end gap-2">
                  <div className="captcha-preview border rounded bg-light d-flex align-items-center justify-content-center">
                    {isCaptchaLoading && (
                      <span className="text-secondary small">Loading...</span>
                    )}
                    {captchaImageUrl && (
                      <img alt="CAPTCHA challenge" src={captchaImageUrl} />
                    )}
                  </div>
                  <button
                    className="btn btn-outline-secondary"
                    onClick={() => {
                      setCaptchaInput("");
                      void createCaptcha();
                    }}
                    type="button"
                  >
                    Refresh
                  </button>
                </div>
              </div>
            </div>

            <div className="modal-footer">
              <button
                className="btn btn-outline-secondary"
                onClick={onClose}
                type="button"
              >
                Cancel
              </button>
              <button className="btn btn-primary" disabled={isSubmitting} type="submit">
                {isSubmitting ? "Saving..." : "Save"}
              </button>
            </div>
          </form>
        </div>
      </div>
      <div className="modal-backdrop fade show" />
    </>
  );
}

function getApiErrorText(error: { name: string } | null | undefined) {
  const name = error?.name.trim();

  return name ? name : null;
}

function getApiErrorFromMutationError(error: unknown) {
  if (!error || typeof error !== "object" || !("data" in error)) {
    return null;
  }

  const data = (error as { data?: unknown }).data;

  if (!data || typeof data !== "object" || !("error" in data)) {
    return null;
  }

  return (data as ApiResponse<unknown>).error;
}

export default CommentFormModal;
