import { useEffect, useLayoutEffect, useRef, useState } from "react";
import { useParams } from "react-router-dom";
import type { AuthUser } from "../types/authUser";
import { getProfileById } from "../services/userService";
import { handleApiError } from "../utils/handleApi";
import ReviewCard from "../components/reviews/ReviewCard";
import CommentCard from "../components/comments/CommentCard";
import type { Review } from "../types/review";
import type { Comment } from "../types/comment";
import { getPagedCommentsForUser } from "../services/commentService";
import { getPagedReviewsForUser } from "../services/reviewService";
import { ChevronLeft, ChevronRight, FileText, MessageSquare, Shield, User } from "lucide-react";

export default function PublicProfile() {
  const { userId } = useParams<{ userId: string }>();

  const [loading, setLoading] = useState(true);
  const [user, setUser] = useState<AuthUser | null>(null);

  const [comments, setComments] = useState<Comment[]>([]);
  const [totalComments, setTotalComments] = useState(0);
  const [commentPage, setCommentPage] = useState(1);
  const [commentPageSize] = useState(4);

  const [reviews, setReviews] = useState<Review[]>([]);
  const [totalReviews, setTotalReviews] = useState(0);
  const [reviewPage, setReviewPage] = useState(1);
  const [reviewPageSize] = useState(4);

  const reviewsRef = useRef<HTMLDivElement | null>(null);
  const commentsRef = useRef<HTMLDivElement | null>(null);

  const [reviewPageChanged, setReviewPageChanged] = useState(false);
  const [commentPageChanged, setCommentPageChanged] = useState(false);

  useEffect(() => {
    if (!userId) return;

    const fetchProfile = async () => {
      setLoading(true);
      try {
        const profile = await getProfileById(userId);
        setUser(profile);
      } catch (err) {
        handleApiError(err);
      }
      finally {
        setLoading(false);
      }
    };

    fetchProfile();
  }, [userId]);

  useEffect(() => {
    if (!userId) return;

    const fetchReviews = async () => {
      try {
        const reviewsData = await getPagedReviewsForUser(userId, { page: reviewPage, pageSize: reviewPageSize });
        setReviews(reviewsData.items);
        setTotalReviews(reviewsData.totalCount);
      } catch (err) {
        handleApiError(err);
      }
    };

    fetchReviews();
  }, [userId, reviewPage]);

  useEffect(() => {
    if (!userId) return;

    const fetchComments = async () => {
      try {
        const commentsData = await getPagedCommentsForUser(userId, { page: commentPage, pageSize: commentPageSize });
        setComments(commentsData.items);
        setTotalComments(commentsData.totalCount);
      } catch (err) {
        handleApiError(err);
      }
    };

    fetchComments();
  }, [userId, commentPage]);

  useLayoutEffect(() => {
    if (reviewPageChanged && reviews.length > 0 && reviewsRef.current) {
      const y = reviewsRef.current.getBoundingClientRect().top + window.scrollY - 60;
      window.scrollTo({ top: y, behavior: "smooth" });
      setReviewPageChanged(false);
    }
  }, [reviews]);

  useLayoutEffect(() => {
    if (commentPageChanged && comments.length > 0 && commentsRef.current) {
      commentsRef.current.scrollIntoView({ behavior: "smooth", block: "start" });
      setCommentPageChanged(false);
    }
  }, [comments]);

  if (loading) {
    return <p className="text-center mt-8">Loading...</p>;
  }

  if (!user) {
    return <p className="text-center mt-8 text-red-500">User not found</p>;
  }

  return (
    <div className="max-w-4xl mx-auto p-8 bg-white/80 backdrop-blur-md rounded-2xl shadow-xl">
      <article className="mb-6">
        <h1 className="text-3xl font-bold mb-4 flex items-center gap-2">
          <User className="w-8 h-8 text-blue-600" />
          {user?.displayName}'s Profile
        </h1>

        <p className="flex items-center gap-2">
          <Shield className={`w-4 h-4 ${
            user.role === "Admin" ? "text-red-600" :
            user.role === "Moderator" ? "text-yellow-600" :
            "text-gray-600"
          }`} />

          <strong>Role:</strong>
          <span
            className={`px-2 py-1 rounded-md text-sm font-semibold ${
              user.role === "Admin" ? "bg-red-100 text-red-600" :
              user.role === "Moderator" ? "bg-yellow-100 text-yellow-700" :
              "bg-gray-100 text-gray-700"
            }`}
          >
            {user.role ?? "User"}
          </span>
        </p>

        <p className="flex items-center gap-2">
          <FileText className="w-4 h-4 text-blue-500" />
          <strong>Reviews count:</strong> {user?.reviews.length}
        </p>
        <p className="flex items-center gap-2">
          <MessageSquare className="w-4 h-4 text-green-500" />
          <strong>Comments count:</strong> {user?.comments.length}
        </p>
      </article>

      <div ref={reviewsRef} className="mb-12 space-y-6">
        <h2 className="text-2xl font-semibold flex items-center gap-2">
          <FileText className="w-6 h-6 text-blue-500" />
          User Reviews
        </h2>
        {reviews.length === 0 ? (
          <p className="text-gray-600">User haven't written any reviews yet</p>
        ) : (
          <div className="grid gap-4 sm:grid-cols-1 md:grid-cols-2">
            {reviews.map((review) => (
              <ReviewCard key={review.id} review={review} />
            ))}
          </div>
        )}

        {totalReviews > reviewPageSize && (
          <div className="flex justify-center items-center gap-2 mt-4">
            <button
              type="button"
              disabled={reviewPage === 1}
              onClick={(e) => {
                e.preventDefault();
                setReviewPage((p) => p - 1);
                setReviewPageChanged(true);
              }}
              className="p-2 rounded bg-gray-200 disabled:opacity-50"
              aria-label="Previous page"
            >
              <ChevronLeft className="w-5 h-5" />
            </button>
            <span className="text-sm">
              Page {reviewPage} of {Math.ceil(totalReviews / reviewPageSize)}
            </span>
            <button
              type="button"
              disabled={reviewPage >= Math.ceil(totalReviews / reviewPageSize)}
              onClick={(e) => {
                e.preventDefault();
                setReviewPage((p) => p + 1);
                setReviewPageChanged(true);
              }}
              className="p-2 rounded bg-gray-200 disabled:opacity-50"
              aria-label="Next page"
            >
              <ChevronRight className="w-5 h-5" />
            </button>
          </div>
        )}
      </div>

      <div ref={commentsRef} className="mb-10  space-y-6">
        <h2 className="text-2xl font-semibold mb-4 flex items-center gap-2">
          <MessageSquare className="w-6 h-6 text-green-600" />
          User Comments
        </h2>
        {comments.length === 0 ? (
          <p className="text-gray-600">User haven't written any comments yet</p>
        ) : (
          <div className="space-y-4">
            {comments.map((comment) => (
              <CommentCard key={comment.id} comment={comment} />
            ))}
          </div>
        )}

        {totalComments > commentPageSize && (
          <div className="flex justify-center items-center gap-2 mt-4">
            <button
              type="button"
              disabled={commentPage === 1}
              onClick={(e) => {
                e.preventDefault();
                setCommentPage((p) => p - 1);
                setCommentPageChanged(true);
              }}
              className="p-2 rounded bg-gray-200 disabled:opacity-50"
              aria-label="Previous page"
            >
              <ChevronLeft className="w-5 h-5" />
            </button>
            <span className="text-sm">
              Page {commentPage} of {Math.ceil(totalComments / commentPageSize)}
            </span>
            <button
              type="button"
              disabled={commentPage >= Math.ceil(totalComments / commentPageSize)}
              onClick={(e) => {
                e.preventDefault();
                setCommentPage((p) => p + 1);
                setCommentPageChanged(true);
              }}
              className="p-2 rounded bg-gray-200 disabled:opacity-50"
              aria-label="Next page"
            >
              <ChevronRight className="w-5 h-5" />
            </button>
          </div>
        )}
      </div>
    </div>
  );
};