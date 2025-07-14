import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import { Link } from 'react-router-dom';
import type { Item } from "../types/item";
import type { Review } from "../types/review";
import { getItemById } from "../services/itemService";
import { getMyReviewForItem, getReviewsForItem, addReview, updateReview, deleteReview } from "../services/reviewService";

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

  const fetchData = async (itemId: string | undefined) => {
    const id = Number(itemId);
    if (!isNaN(id)) {
      getItemById(id)
        .then(setItem)
        .catch((e) => console.error('Failed to fetch item', e))
    
      getReviewsForItem(id)
        .then(setReviews)
        .catch((e) => console.error('Failed to fetch reviews', e))

      if (user) {
        getMyReviewForItem(id)
          .then(data => {
            setMyReview(data)
            setTitle(data.title)
            setContent(data.content)
            setRating(data.rating)
          })
          .catch((e) => console.error('Failed to fetch user review for item', e))
      }
    } else {
      console.error('Invalid item id', itemId);
    }
  };

  useEffect(() => {
    fetchData(id);
  }, [id, user]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    try {
      if (myReview) {
        await updateReview(myReview.id, {title, content, rating});
      } else {
        const itemId = Number(id);
        if (!isNaN(itemId)) {
          console.error('Invalid item id', itemId);
        } else {
          await addReview({itemId, title, content, rating});
        }

        await fetchData(id);
      }
    } catch (e: any) {
      setError(e?.response?.data?.message || "Failed to submit review");
    }
  };

  const handleDelete = async () => {
    var deleteRevId = myReview ? myReview.id : 0; 
    if (deleteRevId == 0) {
      console.error('Invalid review id');
    } else {
      deleteReview(deleteRevId)
        .then(() => {
          setMyReview(null);
          setTitle("");
          setContent("");
          setRating(0);
          fetchData(id);
        })
        .catch((e) => console.error('Failed to delete review', e))
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
            <input
                type="number"
                value={item.id}
                readOnly
                hidden
            />
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