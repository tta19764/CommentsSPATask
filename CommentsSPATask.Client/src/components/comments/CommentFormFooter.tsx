type CommentFormFooterProps = {
  isSubmitDisabled: boolean;
  isSubmitting: boolean;
};

function CommentFormFooter({
  isSubmitDisabled,
  isSubmitting,
}: CommentFormFooterProps) {
  return (
    <div className="modal-footer">
      <button className="btn btn-primary" disabled={isSubmitDisabled} type="submit">
        {isSubmitting ? "Saving..." : "Save"}
      </button>
    </div>
  );
}

export default CommentFormFooter;
