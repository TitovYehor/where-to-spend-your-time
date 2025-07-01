import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

type Review = {
  id: string;
  title: string;
  content: string;
  rating: number;
  createdAt: string;
  author: string;
};

type Item = {
  id: string;
  title: string;
  description: string;
  categoryName: string;
  averageRating: number;
};

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

  const fetchData = async () => {
    const [itemRes, reviewsRes] = await Promise.all([
      fetch(`https://localhost:7005/api/items/${id}`),
      fetch(`https://localhost:7005/api/items/${id}/reviews`),
    ]);

    if (itemRes.ok) {
      const itemData = await itemRes.json();
      setItem(itemData);
    }

    if (reviewsRes.ok) {
      const reviewData = await reviewsRes.json();
      setReviews(reviewData);
    }

    if (user) {
      const myReviewRes = await fetch(
        `https://localhost:7005/api/items/${id}/reviews/my`,
        { credentials: "include" }
      );
      if (myReviewRes.ok) {
        const r = await myReviewRes.json();
        setMyReview(r);
        setTitle(r.title);
        setContent(r.content);
        setRating(r.rating);
      }
    }
  };

  useEffect(() => {
    fetchData();
  }, [id, user]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    const isEditing = !!myReview;
    const method = isEditing ? "PUT" : "POST";
    const url = isEditing
        ? `https://localhost:7005/api/reviews/${myReview.id}`
        : `https://localhost:7005/api/reviews`;

    const body = isEditing
        ? { title, content, rating }
        : { itemId: id, title, content, rating };

    const response = await fetch(url, {
        method,
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(body),
    });

    if (!response.ok) {
        const data = await response.json();
        setError(data.message || "Failed to submit review");
    } else {
        await fetchData();
    }
  };

  const handleDelete = async () => {
    const res = await fetch(
      `https://localhost:7005/api/reviews/${myReview ? myReview.id : 0}`,
      {
        method: "DELETE",
        credentials: "include",
      }
    );

    if (res.ok) {
      setMyReview(null);
      setTitle("");
      setContent("");
      setRating(0);
      await fetchData();
    }
  };

  if (!item) return <div className="p-6">Loading item...</div>;

  return (
    <div className="max-w-3xl mx-auto px-4 py-6">
      <h1 className="text-3xl font-bold mb-2">{item.title}</h1>
      <p className="text-gray-700 mb-6">{item.description}</p>
      <p className="text-gray-700 mb-6">{item.categoryName}</p>
      <p className="text-gray-700 mb-6">{item.averageRating}</p>

      <h2 className="text-xl font-semibold mb-4">Reviews</h2>
      {reviews.length === 0 ? (
        <p className="text-gray-500 mb-6">No reviews yet</p>
      ) : (
        <ul className="space-y-4 mb-6">
          {reviews.map((r) => (
            <li key={r.id} className="bg-white p-4 rounded shadow">
              <div className="flex justify-between mb-1">
                <strong>{r.author}</strong>
                <span className="text-sm text-gray-500">
                  {new Date(r.createdAt).toLocaleDateString()}
                </span>
              </div>
              <div className="font-medium text-yellow-500">Rating: {r.rating}/5</div>
              <h4 className="font-semibold">{r.title}</h4>
              <p>{r.content}</p>
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