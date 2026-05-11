type CommentFormFooterProps = {
  isSubmitDisabled: boolean;
  isSubmitting: boolean;
  onClose: () => void;
};

function CommentFormFooter({
  isSubmitDisabled,
  isSubmitting,
  onClose,
}: CommentFormFooterProps) {
  return (
    <div className="modal-footer">
      <button className="btn btn-outline-secondary" onClick={onClose} type="button">
        Cancel
      </button>
      <button className="btn btn-primary" disabled={isSubmitDisabled} type="submit">
        {isSubmitting ? "Saving..." : "Save"}
      </button>
    </div>
  );
}

export default CommentFormFooter;
