import { Link, useNavigate, useParams } from "react-router-dom";
import { useAutoResizeTextareas } from "../hooks/useAutoResizeTextareas";
import { useEffect, useRef, useState } from "react";
import { useAuth } from "../contexts/AuthContext";
import type { Review } from "../types/review";
import type { Comment } from "../types/comment";
import { deleteReview, getReviewById, updateReview } from "../services/reviewService";
import { addComment, deleteComment, getPagedCommentsForReview } from "../services/commentService";
import { handleApiError } from "../utils/handleApi";
import UserProfileLink from "../components/users/UserProfileLinks";
import { Star, Pencil, Trash2, MessageSquare, PlusCircle, ChevronLeft, ChevronRight, Box } from "lucide-react";
import CommentManagementCard from "../components/comments/CommentManagementCard";
import Alert from "../components/common/Alerts";

export default function ReviewDetails() {
  const { reviewId } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();

  const [review, setReview] = useState<Review | null>(null);
  const [comments, setComments] = useState<Comment[]>([]);
  const [newComment, setNewComment] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");

  const [totalComments, setTotalComments] = useState(0);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(4);

  const [isEditing, setIsEditing] = useState(false);
  const [editTitle, setEditTitle] = useState("");
  const [editContent, setEditContent] = useState("");
  const [editRating, setEditRating] = useState(5);

  const authorReviewRole = review?.authorRole;
  const currentRole = user?.role;

  const isAuthor = user && review?.author === user.displayName;
  const isModerator = currentRole == "Moderator";
  const isAdmin = currentRole == "Admin";

  const id = Number(reviewId);

  const newCommentRef = useRef<HTMLTextAreaElement | null>(null);

  const fetchData = async () => {
    if (isNaN(id)) return;

    try {
      const [reviewData, commentPagedData] = await Promise.all([
        getReviewById(id),
        getPagedCommentsForReview(id, { page, pageSize }),
      ]);
      setReview(reviewData);
      setComments(commentPagedData.items);
      setTotalComments(commentPagedData.totalCount);
    } catch (e) {
      handleApiError(e);
    }
  };

  useAutoResizeTextareas([newCommentRef], [newComment]);

  useEffect(() => {
    fetchData();
  }, [reviewId, user, page]);

  const handleAddComment = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newComment.trim() || isNaN(id)) return;

    try {
      await addComment(id, { content: newComment });
      setMessage("Comment created");
      setNewComment("");
      setPage(1);
      await fetchData();
    } catch (err: any) {
      const message = handleApiError(err);
      setError(message);
      setMessage("");
    }
  };

  const handleDeleteReview = async () => {
    const confirmed = confirm("Are you sure you want to delete this review?");
    if (!confirmed) return;

    try {
      await deleteReview(id);

      navigate(-1);
    } catch (err: any) {
      const message = handleApiError(err);
      setError(message);
    }
  };

  const handleDeleteComment = async (commentId: number) => {
    const canDelete =
      isAdmin ||
      (isModerator && authorReviewRole !== "Admin" && authorReviewRole !== "Moderator") ||
      isAuthor;
    if (!canDelete) return; 

    const confirmed = confirm("Are you sure you want to delete this comment?");
    if (!confirmed) return;

    try {
      await deleteComment(commentId);
      setMessage("Comment deleted");
      await fetchData();
    } catch (err: any) {
      const message = handleApiError(err);
      setError(message);
      setMessage("");
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
      const message = handleApiError(err);
      setError(message);
    }
  };

  return (
    <div className="max-w-4xl mx-auto p-8 bg-white/80 backdrop-blur-md rounded-2xl shadow-xl">
      {review ? (
        <>
          <header className="mb-4 flex flex-col gap-2">
            <Link
              to={`/items/${review.itemId}`}
              className="relative inline-flex w-fit items-center gap-1.5 text-indigo-700 font-semibold text-sm 
                        after:content-[''] after:absolute after:left-0 after:-bottom-0.5 after:w-full after:h-0.5 
                        after:bg-gradient-to-r after:from-indigo-400 after:to-blue-500 
                        after:rounded-full hover:after:h-[3px] transition-all duration-300"
            >
              <Box className="w-4 h-4 text-indigo-600" />
              <span>Reviewed Item: {review.itemTitle}</span>
            </Link>


            <h2 className="text-3xl font-bold mb-1 break-words">{review.title}</h2>
            <p className="text-sm text-black flex items-center gap-1">
              <span>By</span> 
              <UserProfileLink userId={review.userId} name={review.author} />
            </p>
            <p className="text-yellow-600 font-medium flex items-center gap-1">
              <Star className="w-4 h-4" />
              {review.rating}/5
            </p>
          </header>
          
          <section className="mb-6">
            <p className="text-gray-800 break-words">{review.content}</p>
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
                  maxLength={80}
                  required
                />
                <p className="text-xs text-gray-500 mt-1">
                  {editTitle?.length || 0}/80 characters
                </p>
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
                  maxLength={1000}
                  required
                />
                <p className="text-xs text-gray-500 mt-1">
                  {editContent?.length || 0}/1000 characters
                </p>
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
                  className="flex items-center gap-1 bg-green-600 text-white px-4 py-2 rounded-md"
                >
                  <Pencil className="w-4 h-4" />
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
            (isAuthor || isAdmin || ((authorReviewRole != "Admin" && authorReviewRole != "Moderator") && isModerator)) && (
              <div className="flex gap-2 mb-6">
                {isAuthor && (
                  <button
                    onClick={() => {
                      setEditTitle(review.title);
                      setEditContent(review.content);
                      setEditRating(review.rating);
                      setIsEditing(true);
                    }}
                    className="flex items-center gap-1 bg-blue-600 text-white px-4 py-2 rounded-md"
                  >
                    <Pencil className="w-4 h-4" />
                    Edit
                  </button>
                )}
                <button
                  onClick={handleDeleteReview}
                  className="flex items-center gap-1 bg-red-600 text-white px-4 py-2 rounded-md"
                >
                  <Trash2 className="w-4 h-4" />
                  Delete
                </button>
              </div>
            )
          )}

          <Alert type="success" message={message} onClose={() => setMessage("")} />
          <Alert type="error" message={error} onClose={() => setError("")} />
          
          <section>
            <h3 className="text-xl font-semibold mb-3 flex items-center gap-2">
              <MessageSquare className="w-5 h-5 text-green-600" />
              Comments
            </h3>
            {comments.length === 0 ? (
              <p className="text-sm text-gray-500 mb-4">No comments yet</p>
            ) : (
              <>
                <ul className="space-y-4 mb-4">
                  {comments.map((c) => {
                    const authorCommentRole = c.authorRole;
                    const canManage =
                      isAuthor || isAdmin || ((authorCommentRole != "Admin" && authorCommentRole != "Moderator") && isModerator);

                    return (
                      <CommentManagementCard
                        key={c.id}
                        comment={c}
                        canManage={canManage}
                        onDelete={handleDeleteComment}
                      />
                    );
                  })}
                </ul>

                {totalComments > pageSize && (
                  <div className="flex justify-center items-center gap-2 mt-4">
                    <button
                      disabled={page === 1}
                      onClick={() => setPage((p) => p - 1)}
                      className="p-2 rounded bg-gray-200 disabled:opacity-50"
                      aria-label="Previous page"
                    >
                      <ChevronLeft className="w-5 h-5" />
                    </button>
                    <span className="text-sm">
                      Page {page} of {Math.ceil(totalComments / pageSize)}
                    </span>
                    <button
                      disabled={page >= Math.ceil(totalComments / pageSize)}
                      onClick={() => setPage((p) => p + 1)}
                      className="p-2 rounded bg-gray-200 disabled:opacity-50"
                      aria-label="Next page"
                    >
                      <ChevronRight className="w-5 h-5" />
                    </button>
                  </div>
                )}
              </>
            )}

            {user && (
              <form onSubmit={handleAddComment} className="space-y-2">
                <div>
                  <label htmlFor="newComment" className="block text-sm font-medium text-gray-700 mb-1">
                    New comment
                  </label>
                  <textarea
                    ref={newCommentRef}
                    id="newComment"
                    value={newComment}
                    onChange={(e) => {
                      setNewComment(e.target.value);
                    }}
                    className="w-full border px-4 py-2 rounded-md"
                    placeholder="Write a comment..."
                    maxLength={200}
                    required
                  />
                  <p className="text-xs text-gray-500 mt-1">
                    {newComment.length}/200 characters
                  </p>
                </div>

                <div className="flex justify-between items-center">
                  <button
                    type="submit"
                    className="flex items-center gap-1 bg-indigo-600 text-white px-4 py-2 rounded-md"
                  >
                    <PlusCircle className="w-4 h-4" />
                    Add Comment
                  </button>
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