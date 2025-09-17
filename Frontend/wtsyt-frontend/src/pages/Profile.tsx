import { useEffect, useLayoutEffect, useRef, useState } from "react";
import { useAuth } from "../contexts/AuthContext";
import type { Review } from "../types/review";
import type { Comment } from "../types/comment";
import { updatePassword, updateProfile } from "../services/userService";
import { handleApiError } from "../utils/handleApi";
import ReviewCard from "../components/reviews/ReviewCard";
import CommentCard from "../components/comments/CommentCard";
import { getPagedReviewsForUser } from "../services/reviewService";
import { getPagedCommentsForUser } from "../services/commentService";

const Profile = () => {
  const { user, refreshUser } = useAuth();

  const [comments, setComments] = useState<Comment[]>([]);
  const [totalComments, setTotalComments] = useState(0);
  const [commentPage, setCommentPage] = useState(1);
  const [commentPageSize] = useState(4);

  const [reviews, setReviews] = useState<Review[]>([]);
  const [totalReviews, setTotalReviews] = useState(0);
  const [reviewPage, setReviewPage] = useState(1);
  const [reviewPageSize] = useState(4);
  
  const [newDisplayName, setNewDisplayName] = useState(user?.displayName ?? "");
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessages, setErrorMessages] = useState<string[]>([]);

  const reviewsRef = useRef<HTMLDivElement | null>(null);
  const commentsRef = useRef<HTMLDivElement | null>(null);

  const [reviewsLoading, setReviewsLoading] = useState(false);
  const [commentsLoading, setCommentsLoading] = useState(false);

  useEffect(() => {
    if (!user) return;

    const fetchReviews = async () => {
      setReviewsLoading(true);
      try {
        const reviewsData = await getPagedReviewsForUser(user.id, { page: reviewPage, pageSize: reviewPageSize });
        setReviews(reviewsData.items);
        setTotalReviews(reviewsData.totalCount);
      } catch (err) {
        handleApiError(err);
      } finally {
        setReviewsLoading(false);
      }
    };

    fetchReviews();
  }, [user, reviewPage]);

  useEffect(() => {
    if (!user) return;

    const fetchComments = async () => {
      setCommentsLoading(true);
      try {
        const commentsData = await getPagedCommentsForUser(user.id, { page: commentPage, pageSize: commentPageSize });
        setComments(commentsData.items);
        setTotalComments(commentsData.totalCount);
      } catch (err) {
        handleApiError(err);
      } finally {
        setCommentsLoading(false);
      }
    };

    fetchComments();
  }, [user, commentPage]);

  useLayoutEffect(() => {
    if (reviews.length > 0 && reviewsRef.current) {
      const y = reviewsRef.current.getBoundingClientRect().top + window.scrollY - 60;
      window.scrollTo({ top: y, behavior: "smooth" });
    }
  }, [reviews]);

  useLayoutEffect(() => {
    if (comments.length > 0 && commentsRef.current) {
      commentsRef.current.scrollIntoView({ behavior: "smooth", block: "start" });
    }
  }, [comments]);

  if (!user) {
    return <p className="text-center mt-8 text-gray-600">You must be logged in to view this page</p>;
  }

  const handleProfileUpdate = async () => {
    setSuccessMessage("");
    setErrorMessages([]);

    const trimmedName = newDisplayName.trim();

    if (trimmedName.length < 2) {
      setErrorMessages(["Display name must be at least 2 characters."]);
      return;
    }

    if (trimmedName === user?.displayName) {
      return setSuccessMessage("No changes to update.");
    }

    try {
      await updateProfile({ displayName: trimmedName });
      await refreshUser();

      setSuccessMessage("Profile updated successfully!");
    } catch (err: any) {
      handleApiError(err);
      setErrorMessages([
        err?.response?.data?.message || "Failed to update profile.",
      ]);
    }
  };

  const handlePasswordChange = async () => {
      setSuccessMessage("");
      setErrorMessages([]);

      if (!newPassword || newPassword.length < 6) {
        setErrorMessages(["Password must be at least 6 characters."]);
        return;
      }

      try {
        await updatePassword({ currentPassword, newPassword });
        setSuccessMessage("Password changed successfully!");
        setCurrentPassword("");
        setNewPassword("");
      } catch (err: any) {
        handleApiError(err);
        const data = err?.response?.data;
        const errors = Array.isArray(data) ? data : [data?.message || "Failed to change password."];
        setErrorMessages(errors);
      }
  };

  const isDemoAccount = user?.email === "demo@example.com";

  return (
    <div className="max-w-4xl mx-auto p-8 bg-white/80 backdrop-blur-md rounded-2xl shadow-xl">
      <article className="mb-6">
        <h1 className="text-3xl font-bold mb-2">Your Profile</h1>
        <p><strong>Name:</strong> {user?.displayName}</p>
        <p className="mb-3"><strong>Email:</strong> {user?.email ?? "Not available"}</p>
        <p><strong>Reviews count:</strong> {totalReviews}</p>
        <p><strong>Comments count:</strong> {totalComments}</p>
      </article>

      <div className="mb-8 space-y-6">
        <h2 className="text-xl font-semibold">Edit Profile</h2>

        {successMessage && (
          <p className="text-green-600">
            {successMessage}
          </p>
        )}
        {errorMessages.length > 0 && (
          <ul className="mb-4 text-red-500 text-sm list-disc list-inside">
            {errorMessages.map((msg, i) => (
            <li key={i}>{msg}</li>
            ))}
          </ul>
        )}

        <div className="space-y-6">
          <div className="space-y-2">
            <label htmlFor="displayName" className="block font-medium">
              Display Name
            </label>
            <input
              id="displayName"
              value={newDisplayName}
              onChange={(e) => {
                setNewDisplayName(e.target.value)
                setSuccessMessage("");
              }}
              disabled={isDemoAccount}
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition"
            />

            <button
              onClick={handleProfileUpdate}
              disabled={isDemoAccount}
              className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-lg font-semibold transition"
            >
              Save Display Name
            </button>

            {isDemoAccount && (
              <p className="text-sm text-gray-600 mt-1">
                Display name changes are disabled for the demo account.  
                Please register to try this feature.
              </p>
            )}
          </div>
        
          <div className="space-y-2">
            <label htmlFor="currentPassword" className="block font-medium">
              Current Password
            </label>
            <input
              id="currentPassword"
              type="password"
              value={currentPassword}
              onChange={(e) => {
                setCurrentPassword(e.target.value)
                setSuccessMessage("");
              }}
              disabled={isDemoAccount}
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition"
            />

            <label htmlFor="newPassword" className="block font-medium mt-3">
              New Password
            </label>
            <input
              id="newPassword"
              type="password"
              value={newPassword}
              onChange={(e) => {
                setNewPassword(e.target.value)
                setSuccessMessage("");
              }}
              disabled={isDemoAccount}
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition"
            />

            <button
              onClick={handlePasswordChange}
              disabled={isDemoAccount}
              className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-lg font-semibold transition"
            >
              Change Password
            </button>

            {isDemoAccount && (
              <p className="text-sm text-gray-600 mt-1">
                Password changes are disabled for the demo account.  
                Please register to try this feature.
              </p>
            )}
          </div>
        </div>
      </div>

      <div ref={reviewsRef} className="mb-10 space-y-6">
        <h2 className="text-2xl font-semibold mb-4">Your Reviews</h2>

        {reviews.length === 0 ? (
          <p className="text-gray-600">You haven't written any reviews yet</p>
        ) : (
          <div className="grid gap-4 sm:grid-cols-1 md:grid-cols-2">
            {reviews.map((review) => (
              <ReviewCard key={review.id} review={review} />
            ))}

            {reviewsLoading && (
              <div className="absolute inset-0 bg-white/70 flex items-center justify-center rounded">
                <p className="text-gray-500">Loading reviews...</p>
              </div>
            )}
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

      <div ref={commentsRef} className="mb-10 space-y-6">
        <h2 className="text-2xl font-semibold mb-4">Your Comments</h2>

        {comments.length === 0 ? (
          <p className="text-gray-600">You haven't written any comments yet</p>
        ) : (
          <div className="space-y-4">
            {comments.map((comment) => (
              <CommentCard key={comment.id} comment={comment} />
            ))}

            {commentsLoading && (
              <div className="absolute inset-0 bg-white/70 flex items-center justify-center rounded">
                <p className="text-gray-500">Loading comments...</p>
              </div>
            )}
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

export default Profile;
