type CommentCaptchaFieldsProps = {
  captchaImageUrl: string | null;
  captchaInput: string;
  isCaptchaLoading: boolean;
  onRefresh: () => void;
  setCaptchaInput: (value: string) => void;
};

function CommentCaptchaFields({
  captchaImageUrl,
  captchaInput,
  isCaptchaLoading,
  onRefresh,
  setCaptchaInput,
}: CommentCaptchaFieldsProps) {
  return (
    <>
      <div className="col-md-6">
        <label className="form-label" htmlFor="comment-captcha">CAPTCHA</label>
        <input
          aria-describedby="comment-captcha-help"
          className="form-control"
          id="comment-captcha"
          maxLength={10}
          onChange={(event) => setCaptchaInput(event.target.value)}
          pattern="[A-Za-z0-9]+"
          required
          title="Enter the letters and digits shown in the CAPTCHA image."
          value={captchaInput}
        />
        <div className="form-text" id="comment-captcha-help">
          Required. Enter only the Latin letters and digits shown in the image.
        </div>
      </div>

      <div className="col-md-6 d-flex align-items-end gap-2">
        <div className="captcha-preview border rounded bg-light d-flex align-items-center justify-content-center">
          {isCaptchaLoading && <span className="text-secondary small">Loading...</span>}
          {captchaImageUrl && <img alt="CAPTCHA challenge" src={captchaImageUrl} />}
        </div>
        <button className="btn btn-outline-secondary" onClick={onRefresh} type="button">
          Refresh
        </button>
      </div>
    </>
  );
}

export default CommentCaptchaFields;
