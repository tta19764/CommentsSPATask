import { sanitizeCommentHtml } from "../../utils/sanitizeCommentHtml";

type CommentHtmlTextProps = {
  className?: string;
  text: string;
};

function CommentHtmlText({
  className = "comment-text text-secondary mb-3",
  text,
}: CommentHtmlTextProps) {
  return (
    <div
      className={className}
      dangerouslySetInnerHTML={{ __html: sanitizeCommentHtml(text) }}
    />
  );
}

export default CommentHtmlText;
