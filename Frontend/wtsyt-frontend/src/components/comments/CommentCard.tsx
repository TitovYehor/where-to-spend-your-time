import { Link } from "react-router-dom";
import type { Comment } from "../../types/comment";
import { MessageSquare, Calendar } from "lucide-react";

type CommentCardProps = {
  comment: Comment;
};

export default function CommentCard({ comment }: CommentCardProps) {
  return (
    <Link
      to={`/reviews/${comment.reviewId}`}
      className="block border border-gray-200 rounded-xl p-4 hover:shadow-md transition bg-white"
    >
      <p className="text-gray-800 text-sm line-clamp-3 mb-2 break-words flex items-start gap-2">
        <MessageSquare className="w-4 h-4 text-green-600 mt-0.5 flex-shrink-0" />
        {comment.content}
      </p>
      
      <div className="flex items-center text-xs text-violet-600">
        <Calendar className="w-3.5 h-3.5 mr-1" />
        {new Date(comment.createdAt).toLocaleDateString()}
      </div>
    </Link>
  );
}