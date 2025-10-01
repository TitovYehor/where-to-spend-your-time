import { Link } from "react-router-dom";
import type { Review } from "../../types/review";
import { Star, User, Calendar } from "lucide-react";

type ReviewCardProps = {
  review: Review;
};

export default function ReviewCard({ review }: ReviewCardProps) {
  return (
    <Link
      to={`/reviews/${review.id}`}
      className="flex flex-col min-w-0 h-full border border-gray-100 rounded-xl p-4 bg-white shadow-sm hover:shadow-md transition"
    >
      <h3 className="text-lg font-semibold break-words">{review.title}</h3>
      
      <p className="text-sm text-gray-500 flex items-center gap-1 mt-1">
        <User className="w-4 h-4 text-gray-400" />
        {review.author}
      </p>
      
      <div className="mt-auto">
        <p className="mt-2 flex items-center justify-between text-sm text-gray-500">
          <span className="flex items-center gap-1 text-yellow-600 font-medium">
            <Star className="w-4 h-4" />
            {review.rating}/5
          </span>
          <span className="flex items-center gap-1">
            <Calendar className="w-4 h-4 text-gray-400" />
            {new Date(review.createdAt).toLocaleDateString()}
          </span>
        </p>
      </div>
    </Link>
  );
}