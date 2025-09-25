import { Link } from "react-router-dom";
import type { Comment } from "../../types/comment";

type CommentCardProps = {
  comment: Comment;
};

export default function CommentCard({ comment }: CommentCardProps) {
  return (
    <Link
      to={`/reviews/${comment.reviewId}`}
      className="block border border-gray-200 rounded-xl p-4 hover:shadow-md transition bg-white"
    >
      <p className="text-gray-800 text-sm line-clamp-3 mb-2 break-words">{comment.content}</p>
      <p className="text-xs text-gray-500">{new Date(comment.createdAt).toLocaleDateString()}</p>
    </Link>
  );
}