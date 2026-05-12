import { useEffect, useRef } from "react";
import {
  sanitizeCommentHtml,
  validateCommentHtml,
} from "../../utils/sanitizeCommentHtml";

type CommentTextEditorProps = {
  setText: (value: string | ((currentText: string) => string)) => void;
  text: string;
};

const allowedTags = [
  { label: "a", value: '<a href="" title=""></a>' },
  { label: "code", value: "<code></code>" },
  { label: "i", value: "<i></i>" },
  { label: "strong", value: "<strong></strong>" },
];

function CommentTextEditor({ setText, text }: CommentTextEditorProps) {
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const validationErrors = Array.from(new Set(validateCommentHtml(text)));
  const hasValidationErrors = validationErrors.length > 0;
  const previewHtml = sanitizeCommentHtml(text);

  useEffect(() => {
    const htmlMessage = validationErrors.join(" ");
    const lengthMessage =
      text.length > 2500 ? "Comment text must be 2500 characters or fewer." : "";

    textareaRef.current?.setCustomValidity(htmlMessage || lengthMessage);
  }, [text, validationErrors]);

  const insertTag = (tagValue: string) => {
    setText((currentText) => `${currentText}${tagValue}`);
  };

  return (
    <div className="col-12">
      <div className="d-flex flex-column flex-md-row justify-content-between gap-2 align-items-md-end">
        <label className="form-label" htmlFor="comment-text">Text</label>
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
        aria-invalid={hasValidationErrors}
        aria-describedby="comment-text-help"
        className={`form-control ${hasValidationErrors ? "is-invalid" : ""}`}
        id="comment-text"
        maxLength={2500}
        onChange={(event) => setText(event.target.value)}
        ref={textareaRef}
        required
        rows={5}
        value={text}
      />
      <div className="form-text" id="comment-text-help">
        Required. Up to 2500 characters. Only [a], [code], [i], and [strong]
        HTML tags are allowed.
      </div>
      {hasValidationErrors && (
        <div className="invalid-feedback d-block">
          {validationErrors.join(" ")}
        </div>
      )}

      <div className="mt-3">
        <div className="d-flex justify-content-between align-items-center mb-1">
          <span className="form-label mb-0">Preview</span>
          <span className={hasValidationErrors ? "text-danger small" : "text-secondary small"}>
            {hasValidationErrors ? "Contains invalid HTML" : "Allowed HTML only"}
          </span>
        </div>
        <div className="comment-preview border rounded bg-light p-3">
          {text.trim() ? (
            <div
              className="comment-text mb-0"
              dangerouslySetInnerHTML={{ __html: previewHtml }}
            />
          ) : (
            <span className="text-secondary">Message preview will appear here.</span>
          )}
        </div>
      </div>
    </div>
  );
}

export default CommentTextEditor;
