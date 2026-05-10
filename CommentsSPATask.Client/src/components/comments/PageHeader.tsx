type PageHeaderProps = {
  totalCount: number;
  onAddComment: () => void;
};

function PageHeader({ totalCount, onAddComment }: PageHeaderProps) {
  return (
    <section className="d-flex flex-column flex-md-row justify-content-between gap-3 align-items-md-end mb-4">
      <div>
        <p className="text-primary fw-semibold text-uppercase small mb-1">
          Comments SPA
        </p>
        <h1 className="display-5 fw-semibold mb-0">Comments</h1>
      </div>

      <div className="d-flex flex-column flex-sm-row align-items-stretch align-items-sm-end gap-2">
        <button className="btn btn-primary" onClick={onAddComment} type="button">
          Add comment
        </button>

        <div className="card border-primary-subtle">
          <div className="card-body py-3 px-4 text-sm-end">
            <strong className="fs-3 d-block">{totalCount}</strong>
            <span className="text-secondary">root comments</span>
          </div>
        </div>
      </div>
    </section>
  );
}

export default PageHeader;
