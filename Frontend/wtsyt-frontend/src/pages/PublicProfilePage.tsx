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
    if (reviews.length > 0 && reviewsRef.current) {
      const y = reviewsRef.current.getBoundingClientRect().top + window.scrollY - 60;
      window.scrollTo({ top: y, behavior: "smooth" });
    }
  }, [reviews]);

  useLayoutEffect(() => {
    if (comments.length > 0) {
      commentsRef.current?.scrollIntoView({ behavior: "smooth", block: "start" });
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
      <article className="mb-10">
        <h1 className="text-3xl font-bold mb-2">{user?.displayName}'s Profile</h1>
        <p><strong>Reviews count:</strong> {user?.reviews.length}</p>
        <p><strong>Comments count:</strong> {user?.comments.length}</p>
      </article>

      <div ref={reviewsRef} className="mb-12 space-y-6">
        <h2 className="text-2xl font-semibold">User Reviews</h2>
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
              }}
              className="px-3 py-1 bg-gray-200 rounded disabled:opacity-50"
            >
              Prev
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
              }}
              className="px-3 py-1 bg-gray-200 rounded disabled:opacity-50"
            >
              Next
            </button>
          </div>
        )}
      </div>

      <div ref={commentsRef} className="mb-10  space-y-6">
        <h2 className="text-2xl font-semibold mb-4">User Comments</h2>
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
              }}
              className="px-3 py-1 bg-gray-200 rounded disabled:opacity-50"
            >
              Prev
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
              }}
              className="px-3 py-1 bg-gray-200 rounded disabled:opacity-50"
            >
              Next
            </button>
          </div>
        )}
      </div>
    </div>
  );
};