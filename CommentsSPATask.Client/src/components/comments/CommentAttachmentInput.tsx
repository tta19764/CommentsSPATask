type CommentAttachmentInputProps = {
  attachmentError: string | null;
  onAttachmentChange: (file: File | undefined) => void;
};

function CommentAttachmentInput({
  attachmentError,
  onAttachmentChange,
}: CommentAttachmentInputProps) {
  return (
    <div className="col-12">
      <label className="form-label" htmlFor="comment-attachment">Attachment</label>
      <input
        accept=".jpg,.jpeg,.png,.gif,.txt,image/jpeg,image/png,image/gif,text/plain"
        aria-describedby="comment-attachment-help"
        className={`form-control ${attachmentError ? "is-invalid" : ""}`}
        id="comment-attachment"
        onChange={(event) =>
          onAttachmentChange(event.target.files?.[0] ?? undefined)
        }
        type="file"
      />
      <div className="form-text" id="comment-attachment-help">
        Optional. Allowed files: JPG, JPEG, PNG, GIF, or TXT. Text files must be
        100 KB or smaller; large images are resized by the server.
      </div>
      {attachmentError && (
        <div className="invalid-feedback d-block">{attachmentError}</div>
      )}
    </div>
  );
}

export default CommentAttachmentInput;
