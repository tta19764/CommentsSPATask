type CommentsPaginationProps = {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
};

function CommentsPagination({
  currentPage,
  totalPages,
  onPageChange,
}: CommentsPaginationProps) {
  const pages = Array.from({ length: totalPages }, (_, index) => index + 1)
    .filter((page) => {
      return (
        page === 1 ||
        page === totalPages ||
        Math.abs(page - currentPage) <= 1
      );
    });

  return (
    <nav aria-label="Comments pagination" className="mt-4">
      <ul className="pagination justify-content-center flex-wrap gap-1">
        <li className="page-item">
          <button
            className="page-link"
            disabled={currentPage <= 1}
            onClick={() => onPageChange(currentPage - 1)}
            type="button"
          >
            Previous
          </button>
        </li>

        {pages.map((page, index) => {
          const previousPage = pages[index - 1];
          const needsGap = previousPage && page - previousPage > 1;

          return (
            <li className="page-item d-flex align-items-center" key={page}>
              {needsGap && <span className="px-2 text-secondary">...</span>}
              <button
                className={`page-link ${page === currentPage ? "active" : ""}`}
                onClick={() => onPageChange(page)}
                type="button"
              >
                {page}
              </button>
            </li>
          );
        })}

        <li className="page-item">
          <button
            className="page-link"
            disabled={currentPage >= totalPages}
            onClick={() => onPageChange(currentPage + 1)}
            type="button"
          >
            Next
          </button>
        </li>
      </ul>
    </nav>
  );
}

export default CommentsPagination;
