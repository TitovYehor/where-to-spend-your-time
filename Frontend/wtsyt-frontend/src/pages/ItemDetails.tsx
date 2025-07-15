import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import { Link } from 'react-router-dom';
import type { Item } from "../types/item";
import type { Review } from "../types/review";
import { getItemById } from "../services/itemService";
import { getMyReviewForItem, getReviewsForItem, addReview, updateReview, deleteReview } from "../services/reviewService";
import { handleApiError } from "../utils/handleApi";

export default function ItemDetails() {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuth();

  const [item, setItem] = useState<Item | null>(null);
  const [reviews, setReviews] = useState<Review[]>([]);
  const [myReview, setMyReview] = useState<Review | null>(null);

  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [rating, setRating] = useState(0);
  const [error, setError] = useState("");

  const itemId = Number(id);

  const fetchData = async () => {
    if (isNaN(itemId)) {
      console.error("Invalid item id", id);
      return;
    }

    try {
      const [itemData, reviewsData] = await Promise.all([
        getItemById(itemId),
        getReviewsForItem(itemId),
      ]);
      setItem(itemData);
      setReviews(reviewsData);
    } catch (e) {
      handleApiError(e);
      return;
    }

    if (user) {
      try {
        const myReviewData = await getMyReviewForItem(itemId);
        setMyReview(myReviewData);
        setTitle(myReviewData.title);
        setContent(myReviewData.content);
        setRating(myReviewData.rating);
      } catch {
        setMyReview(null);
        setTitle("");
        setContent("");
        setRating(0);
      }
    }
  };

  useEffect(() => {
    fetchData();
  }, [id, user]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    if (isNaN(itemId)) {
      setError("Invalid item ID");
      return;
    }

    try {
      if (myReview) {
        await updateReview(myReview.id, { title, content, rating });
      } else {
        await addReview({ itemId, title, content, rating });
      }

      await fetchData();
    } catch (e: any) {
      setError(e?.response?.data?.message || "Failed to submit review");
    }
  };

  const handleDelete = async () => {
    if (!myReview) return;

    try {
      await deleteReview(myReview.id);
      setMyReview(null);
      setTitle("");
      setContent("");
      setRating(0);

      await fetchData();
    } catch (e) {
      handleApiError(e);
    }
  };

  if (!item) return <div className="p-6">Loading item...</div>;

  return (
    <div className="max-w-3xl mx-auto px-4 py-6">
      <h1 className="text-3xl font-bold mb-2">Title: {item.title}</h1>
      <p className="text-gray-700 mb-6">Description: {item.description}</p>
      <p className="text-gray-700 mb-6">Category: {item.categoryName}</p>
      <p className="text-gray-700 mb-6">Average rating: {item.averageRating}</p>

      <h2 className="text-xl font-semibold mb-4">Reviews</h2>
      {reviews.length === 0 ? (
        <p className="text-gray-500 mb-6">No reviews yet</p>
      ) : (
        <ul className="space-y-4 mb-6">
          {reviews.map((review) => (
            <li key={review.id}>
              <Link 
                to={`/reviews/${review.id}`}
                className="block p-4 bg-white rounded shadow hover:shadow-md transition"
              >
                <h3 className="text-lg font-semibold">{review.title}</h3>
                <p className="text-sm text-gray-600">Content: {review.content}</p>
                <p className="text-yellow-500">Rating: {review.rating}/5</p>
                <p className="text-sm text-gray-500">
                  {new Date(review.createdAt).toLocaleDateString()}
                </p>
              </Link>
            </li>
          ))}
        </ul>
      )}

      {user && (
        <div className="bg-white p-6 rounded-lg shadow">
          <h3 className="text-lg font-bold mb-4">
            {myReview ? "Edit your review" : "Write a review"}
          </h3>
          {error && <p className="text-red-500 mb-2">{error}</p>}
          <form onSubmit={handleSubmit} className="space-y-3">
            <input type="hidden" value={item.id} readOnly />
            <input
              type="text"
              placeholder="Title"
              className="w-full border rounded px-3 py-2"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              required
            />
            <textarea
              placeholder="Content"
              className="w-full border rounded px-3 py-2"
              value={content}
              onChange={(e) => setContent(e.target.value)}
              required
            />
            <input
              type="number"
              min="1"
              max="5"
              className="w-full border rounded px-3 py-2"
              placeholder="Rating (1-5)"
              value={rating}
              onChange={(e) => setRating(Number(e.target.value))}
              required
            />
            <button
              type="submit"
              className="bg-indigo-600 text-white py-2 px-4 rounded hover:bg-indigo-700"
            >
              {myReview ? "Update Review" : "Submit Review"}
            </button>
            {myReview && (
              <button
                type="button"
                className="ml-4 text-red-600 hover:underline"
                onClick={handleDelete}
              >
                Delete Review
              </button>
            )}
          </form>
        </div>
      )}
    </div>
  );
}