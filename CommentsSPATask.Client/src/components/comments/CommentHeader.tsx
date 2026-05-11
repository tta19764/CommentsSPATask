import type { MouseEvent } from "react";
import type { Comment } from "../../types/comments/commentType";
import { formatDate } from "../../utils/formatDate";

type CommentHeaderProps = {
  comment: Comment;
  titleClassName?: string;
  stopPropagation?: boolean;
};

function CommentHeader({
  comment,
  titleClassName = "h5 mb-1",
  stopPropagation = false,
}: CommentHeaderProps) {
  const handleClick = stopPropagation
    ? (event: MouseEvent<HTMLAnchorElement>) => event.stopPropagation()
    : undefined;

  return (
    <header className="d-flex flex-column flex-md-row justify-content-between gap-2 mb-3">
      <div>
        <div className={titleClassName}>
          {comment.homePage ? (
            <a
              href={comment.homePage}
              onClick={handleClick}
              rel="noreferrer"
              target="_blank"
            >
              {comment.userName}
            </a>
          ) : (
            comment.userName
          )}
        </div>
        <a href={`mailto:${comment.email}`} onClick={handleClick}>
          {comment.email}
        </a>
      </div>
      <time className="text-secondary small" dateTime={comment.createdAtUtc}>
        {formatDate(comment.createdAtUtc)}
      </time>
    </header>
  );
}

export default CommentHeader;
