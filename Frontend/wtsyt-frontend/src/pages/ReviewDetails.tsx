import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { useAuth } from "../contexts/AuthContext";

type Review = {
  id: number;
  title: string;
  content: string;
  rating: number;
  createdAt: string;
  author: string;
};

type Comment = {
  id: number;
  content: string;
  author: string;
  createdAt: string;
};

export default function ReviewDetails() {
  const { reviewId } = useParams();
  const { user } = useAuth();

  const [review, setReview] = useState<Review | null>(null);
  const [comments, setComments] = useState<Comment[]>([]);
  const [newComment, setNewComment] = useState("");
  const [error, setError] = useState("");

  const [isEditing, setIsEditing] = useState(false);
  const [editTitle, setEditTitle] = useState("");
  const [editContent, setEditContent] = useState("");
  const [editRating, setEditRating] = useState(5);

  const fetchData = async () => {
    const res = await fetch(`https://localhost:7005/api/reviews/${reviewId}`, {
        credentials: "include",
    }
    );
    if (res.ok) {
      const data = await res.json();
      setReview(data);
    }

    const commentsRes = await fetch(`https://localhost:7005/api/reviews/${reviewId}/comments`, {
        credentials: "include",
    }
    );
    if (commentsRes.ok) {
      const data = await commentsRes.json();
      setComments(data);
    }
  };

  useEffect(() => {
    fetchData();
  }, [reviewId]);

  const handleAddComment = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newComment.trim()) return;

    const res = await fetch(`https://localhost:7005/api/reviews/${review?.id}/comments`, {
      method: "POST",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ reviewId, content: newComment }),
    });

    if (res.ok) {
      setNewComment("");
      fetchData();
    } else {
      const err = await res.json();
      setError(err.message || "Failed to add comment");
    }
  };

  const handleDeleteReview = async () => {
    const confirmed = confirm("Are you sure you want to delete this review?");
    if (!confirmed) return;

    const res = await fetch(`https://localhost:7005/api/reviews/${reviewId}`, {
      method: "DELETE",
      credentials: "include",
    });

    if (res.ok) {
      window.location.href = "/items";
    }
  };

  const isAuthor = user && review?.author === user.displayName;
  const isAdmin = user?.email === "admin@example.com";

  return (
    <div className="max-w-2xl mx-auto p-4 bg-white rounded-xl shadow">
      {review ? (
        <>
          <h2 className="text-2xl font-bold">{review.title}</h2>
          <p className="text-gray-600 mb-1">By {review.author}</p>
          <p className="text-yellow-500 mb-2">Rating: {review.rating}/5</p>
          <p className="mb-4">{review.content}</p>
          {isEditing && (
            <form
                onSubmit={async (e) => {
                e.preventDefault();

                const res = await fetch(`https://localhost:7005/api/reviews/${reviewId}`, {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    credentials: "include",
                    body: JSON.stringify({
                    title: editTitle,
                    content: editContent,
                    rating: editRating,
                    }),
                });

                if (res.ok) {
                    setIsEditing(false);
                    fetchData();
                } else {
                    const err = await res.json();
                    alert(err.message || "Failed to update review");
                }
                }}
                className="space-y-3 mt-4"
            >
                <input
                    type="text"
                    className="w-full border px-3 py-2 rounded"
                    value={editTitle}
                    onChange={(e) => setEditTitle(e.target.value)}
                    required
                />
                <textarea
                    className="w-full border px-3 py-2 rounded"
                    value={editContent}
                    onChange={(e) => setEditContent(e.target.value)}
                    required
                />
                <input
                    type="number"
                    min={1}
                    max={5}
                    className="w-full border px-3 py-2 rounded"
                    value={editRating}
                    onChange={(e) => setEditRating(Number(e.target.value))}
                    required
                />
                <div className="flex gap-2">
                    <button type="submit" className="bg-green-600 text-white px-4 py-1 rounded">
                        Save
                    </button>
                    <button
                        type="button"
                        onClick={() => setIsEditing(false)}
                        className="bg-gray-300 text-black px-4 py-1 rounded"
                    >
                        Cancel
                    </button>
                </div>
            </form>
          )}
          {(isAuthor || isAdmin) && !isEditing && (
            <div className="space-x-2 mb-6">
              {isAuthor && (
                <button
                  onClick={() => {
                    setEditTitle(review.title);
                    setEditContent(review.content);
                    setEditRating(review.rating);
                    setIsEditing(true);
                  }}
                  className="bg-blue-600 text-white px-4 py-1 rounded"
                >
                  Edit
                </button>
              )}
              <button
                onClick={handleDeleteReview}
                className="bg-red-600 text-white px-4 py-1 rounded"
              >
                Delete
              </button>
            </div>
          )}

          <h3 className="text-lg font-semibold mt-6">Comments</h3>
          <ul className="space-y-2 mt-2">
            {comments.map((c) => (
              <li key={c.id} className="border p-2 rounded">
                <p className="text-sm text-gray-600">
                  {c.author} • {new Date(c.createdAt).toLocaleString()}
                </p>
                <p>{c.content}</p>
              </li>
            ))}
          </ul>

          {user && (
            <form onSubmit={handleAddComment} className="mt-4 space-y-2">
              <textarea
                value={newComment}
                onChange={(e) => setNewComment(e.target.value)}
                className="w-full border rounded px-3 py-2"
                placeholder="Write a comment..."
                required
              />
              <button
                type="submit"
                className="bg-indigo-600 text-white px-4 py-1 rounded"
              >
                Add Comment
              </button>
              {error && <p className="text-red-500">{error}</p>}
            </form>
          )}
        </>
      ) : (
        <p>Loading review...</p>
      )}
    </div>
  );
}