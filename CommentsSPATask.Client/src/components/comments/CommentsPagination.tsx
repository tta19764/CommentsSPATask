import { Link } from "react-router-dom";

type CommentsPaginationProps = {
  currentPage: number;
  totalPages: number;
  getPageUrl: (page: number) => string;
};

function CommentsPagination({
  currentPage,
  totalPages,
  getPageUrl,
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
        <li className={`page-item ${currentPage <= 1 ? "disabled" : ""}`}>
          {currentPage <= 1 ? (
            <span className="page-link">Previous</span>
          ) : (
            <Link className="page-link" to={getPageUrl(currentPage - 1)}>
              Previous
            </Link>
          )}
        </li>

        {pages.map((page, index) => {
          const previousPage = pages[index - 1];
          const needsGap = previousPage && page - previousPage > 1;

          return (
            <li className="page-item d-flex align-items-center" key={page}>
              {needsGap && <span className="px-2 text-secondary">...</span>}
              <Link
                className={`page-link ${page === currentPage ? "active" : ""}`}
                to={getPageUrl(page)}
              >
                {page}
              </Link>
            </li>
          );
        })}

        <li className={`page-item ${currentPage >= totalPages ? "disabled" : ""}`}>
          {currentPage >= totalPages ? (
            <span className="page-link">Next</span>
          ) : (
            <Link className="page-link" to={getPageUrl(currentPage + 1)}>
              Next
            </Link>
          )}
        </li>
      </ul>
    </nav>
  );
}

export default CommentsPagination;
