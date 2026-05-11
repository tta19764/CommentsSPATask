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
        className="form-control"
        id="comment-text"
        onChange={(event) => setText(event.target.value)}
        required
        rows={5}
        value={text}
      />
    </div>
  );
}

export default CommentTextEditor;
