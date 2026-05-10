import {
  CommentSortField,
  SortDirection,
  type CommentSortField as CommentSortFieldValue,
  type SortDirection as SortDirectionValue,
} from "../../types/comments/getCommentsRequestType";

const sortFieldLabels: Record<CommentSortFieldValue, string> = {
  [CommentSortField.UserName]: "User name",
  [CommentSortField.Email]: "E-mail",
  [CommentSortField.CreatedAtUtc]: "Date",
};

type SortToolbarProps = {
  sortField: CommentSortFieldValue;
  sortDirection: SortDirectionValue;
  onSortChange: (sortField: CommentSortFieldValue) => void;
};

function SortToolbar({
  sortField,
  sortDirection,
  onSortChange,
}: SortToolbarProps) {
  return (
    <section
      aria-label="Comment sorting controls"
      className="card mb-3"
    >
      <div className="card-body d-flex flex-column flex-md-row justify-content-between gap-3 align-items-md-center">
        <span className="text-secondary">Sort by</span>

        <div className="btn-group flex-wrap" role="group">
          {Object.values(CommentSortField).map((field) => {
            const isActive = field === sortField;

            return (
              <button
                className={`btn ${isActive ? "btn-primary" : "btn-outline-primary"}`}
                key={field}
                onClick={() => onSortChange(field)}
                type="button"
              >
                {sortFieldLabels[field]}
                {isActive && (
                  <span aria-hidden="true">
                    {sortDirection === SortDirection.Asc ? " ↑" : " ↓"}
                  </span>
                )}
              </button>
            );
          })}
        </div>
      </div>
    </section>
  );
}

export default SortToolbar;
