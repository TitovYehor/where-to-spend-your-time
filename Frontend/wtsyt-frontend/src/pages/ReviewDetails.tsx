import { Link, useNavigate, useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { useAuth } from "../contexts/AuthContext";
import type { Review } from "../types/review";
import type { Comment } from "../types/comment";
import { deleteReview, getReviewById, updateReview } from "../services/reviewService";
import { getCommentsForReview, addComment, deleteComment } from "../services/commentService";
import { handleApiError } from "../utils/handleApi";

export default function ReviewDetails() {
  const { reviewId } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();

  const [review, setReview] = useState<Review | null>(null);
  const [comments, setComments] = useState<Comment[]>([]);
  const [newComment, setNewComment] = useState("");
  const [error, setError] = useState("");

  const [isEditing, setIsEditing] = useState(false);
  const [editTitle, setEditTitle] = useState("");
  const [editContent, setEditContent] = useState("");
  const [editRating, setEditRating] = useState(5);

  const isAuthor = user && review?.author === user.displayName;
  const isAdmin = user?.role == "Admin";

  const id = Number(reviewId);

  const fetchData = async () => {
    if (isNaN(id)) return;

    try {
      const [reviewData, commentData] = await Promise.all([
        getReviewById(id),
        getCommentsForReview(id),
      ]);
      setReview(reviewData);
      setComments(commentData);
    } catch (e) {
      handleApiError(e);
    }
  };

  useEffect(() => {
    fetchData();
  }, [reviewId, user]);

  const handleAddComment = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newComment.trim() || isNaN(id)) return;

    try {
      await addComment(id, { content: newComment });
      setNewComment("");

      await fetchData();
    } catch (err: any) {
      handleApiError(err);
      setError(err.message || "Failed to add comment");
    }
  };

  const handleDeleteReview = async () => {
    const confirmed = confirm("Are you sure you want to delete this review?");
    if (!confirmed) return;

    try {
      await deleteReview(id);

      navigate("/items");
    } catch (err: any) {
      handleApiError(err);
      setError(err.message || "Failed to delete review");
    }
  };

  const handleDeleteComment = async (commentId: number, commentAuthor: string) => {
    const canDelete = isAdmin || commentAuthor === user?.displayName;
    if (!canDelete) return;

    const confirmed = confirm("Are you sure you want to delete this comment?");
    if (!confirmed) return;

    try {
      await deleteComment(commentId);
      
      await fetchData();
    } catch (err: any) {
      handleApiError(err);
      setError(err.message || "Failed to delete comment");
    }
  };

  const handleEditSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (isNaN(id)) return;

    try {
      await updateReview(id, {
        title: editTitle,
        content: editContent,
        rating: editRating,
      });
      setIsEditing(false);
      fetchData();
    } catch (err: any) {
      handleApiError(err);
      setError(err.message || "Failed to update review");
    }
  };

  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8 bg-white/60 backdrop-blur-md rounded-xl shadow-lg">
      {review ? (
        <>
          <header className="mb-4">
            <h2 className="text-3xl font-bold mb-1">{review.title}</h2>
            <p className="text-sm text-black flex items-center gap-1">
              <span>By</span> 
              <Link 
                to={`/users/${review.userId}`} 
                className="text-blue-700 hover:underline"
              >
                {review.author}
              </Link>
            </p>
            <p className="text-yellow-600 font-medium">Rating: {review.rating}/5</p>
          </header>
          
          <section className="mb-6">
            <p className="text-gray-800">{review.content}</p>
          </section>

          {isEditing ? (
            <form onSubmit={handleEditSubmit} className="space-y-3 mb-6">
              <div>
                <label htmlFor="editTitle" className="block text-sm font-medium text-gray-700 mb-1">
                  Edit title
                </label>
                <input
                  id="editTitle"
                  type="text"
                  className="w-full border px-4 py-2 rounded-md"
                  value={editTitle}
                  onChange={(e) => setEditTitle(e.target.value)}
                  required
                />
              </div>

              <div>
                <label htmlFor="editContent" className="block text-sm font-medium text-gray-700 mb-1">
                  Edit content
                </label>
                <textarea
                  id="editContent"
                  className="w-full border px-3 py-2 rounded"
                  value={editContent}
                  onChange={(e) => setEditContent(e.target.value)}
                  required
                />
              </div>

              <div>
                <label htmlFor="editRating" className="block text-sm font-medium text-gray-700 mb-1">
                  Edit rating
                </label>
                <input
                  id="editRating"
                  type="number"
                  min={1}
                  max={5}
                  className="w-full border px-3 py-2 rounded"
                  value={editRating}
                  onChange={(e) => setEditRating(Number(e.target.value))}
                  required
                />
              </div>

              <div className="flex gap-2">
                <button
                  type="submit"
                  className="bg-green-600 text-white px-4 py-2 rounded-md"
                >
                  Save
                </button>
                <button
                  type="button"
                  onClick={() => setIsEditing(false)}
                  className="bg-gray-300 text-black px-4 py-2 rounded-md"
                >
                  Cancel
                </button>
              </div>
            </form>
          ) : ( 
            (isAuthor || isAdmin) && (
              <div className="flex gap-2 mb-6">
                {isAuthor && (
                  <button
                    onClick={() => {
                      setEditTitle(review.title);
                      setEditContent(review.content);
                      setEditRating(review.rating);
                      setIsEditing(true);
                    }}
                    className="bg-blue-600 text-white px-4 py-2 rounded-md"
                  >
                    Edit
                  </button>
                )}
                <button
                  onClick={handleDeleteReview}
                  className="bg-red-600 text-white px-4 py-2 rounded-md"
                >
                  Delete
                </button>
              </div>
            )
          )}
          
          <section>
            <h3 className="text-xl font-semibold mb-3">Comments</h3>
            {comments.length === 0 ? (
              <p className="text-sm text-gray-500 mb-4">No comments yet.</p>
            ) : (
              <ul className="space-y-4 mb-4">
                {comments.map((c) => (
                  <li 
                    key={c.id} 
                    className="border rounded-lg p-3 bg-gray-50 shadow-sm"
                  >
                    <div className="flex justify-between items-center mb-1">
                      <p className="text-sm text-gray-600 flex items-center gap-1"> 
                        <Link 
                          to={`/users/${c.userId}`} 
                          className="text-blue-600 hover:underline"
                        >
                          {c.author}
                        </Link>
                        <span>• {new Date(c.createdAt).toLocaleString()}</span>
                      </p>
                      {(isAdmin || c.author === user?.displayName) && (
                        <button
                          onClick={() => handleDeleteComment(c.id, c.author)}
                          className="text-sm text-white bg-red-600 hover:bg-red-700 px-3 py-1 rounded-md"
                        >
                          Delete
                        </button>
                      )}
                    </div>
                    <p className="text-gray-800">{c.content}</p>
                  </li>
                ))}
              </ul>
            )}

            {user && (
              <form onSubmit={handleAddComment} className="space-y-2">
                <div>
                  <label htmlFor="newComment" className="block text-sm font-medium text-gray-700 mb-1">
                    New comment
                  </label>
                  <textarea
                    id="newComment"
                    value={newComment}
                    onChange={(e) => setNewComment(e.target.value)}
                    className="w-full border px-4 py-2 rounded-md"
                    placeholder="Write a comment..."
                    required
                  />
                </div>

                <div className="flex justify-between items-center">
                  <button
                    type="submit"
                    className="bg-indigo-600 text-white px-4 py-2 rounded-md"
                  >
                    Add Comment
                  </button>
                  {error && <p className="text-red-500 text-sm">{error}</p>}
                </div>
              </form>
            )}
          </section>
        </>
      ) : (
        <p className="text-center text-gray-500">Loading review...</p>
      )}
    </div>
  );
}