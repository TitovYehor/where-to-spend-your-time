import { Link } from "react-router-dom";
import type { Review } from "../../types/review";

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
      <p className="text-sm text-gray-500">By {review.author}</p>
      <div className="mt-auto">
        <p className="mt-2 flex items-center justify-between text-sm text-gray-500">
          <span className="text-yellow-500 font-medium">
              Rating: {review.rating}/5
          </span>
          <span>
              {new Date(review.createdAt).toLocaleDateString()}
          </span>
        </p>
      </div>
    </Link>
  );
}