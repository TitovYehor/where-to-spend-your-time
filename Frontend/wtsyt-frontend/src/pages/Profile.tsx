import { useEffect, useState } from "react";
import { useAuth } from "../contexts/AuthContext";
import { Link } from "react-router-dom";
import type { Review } from "../types/review";
import type { Comment } from "../types/comment";
import { getMyProfile } from "../services/userService";
import { updatePassword, updateProfile } from "../services/userService";
import { handleApiError } from "../utils/handleApi";

const Profile = () => {
  const { user, refreshUser } = useAuth();
  const [loading, setLoading] = useState(true);

  const [reviews, setReviews] = useState<Review[]>([]);
  const [comments, setComments] = useState<Comment[]>([]);
  
  const [newDisplayName, setNewDisplayName] = useState(user?.displayName ?? "");
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  
  const [successMessage, setSuccessMessage] = useState("");
  const [errorMessages, setErrorMessages] = useState<string[]>([]);

  useEffect(() => {
    const fetchProfile = async () => {
      setLoading(true);
      try {
        const data = await getMyProfile();
        setReviews(data.reviews);
        setComments(data.comments);
      } catch (err) {
        console.error("Failed to fetch profile", err);
      } finally {
        setLoading(false);
      }
    };

    if (user) {
      fetchProfile();
    } else {
      setLoading(false);
    }
  }, [user]);

  if (!user && !loading) {
    return <p className="text-center mt-8 text-gray-600">You must be logged in to view this page</p>;
  }

  if (loading) {
    return <p className="text-center mt-8">Loading...</p>;
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

  return (
    <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <h1 className="text-3xl font-bold mb-6">Your Profile</h1>

      <div className="mb-8 bg-white shadow rounded-2xl p-6 space-y-3">
        <p><strong>Name:</strong> {user?.displayName}</p>
        <p><strong>Email:</strong> {user?.email ?? "Not available"}</p>
      </div>

      <div className="mb-8 bg-white p-6 rounded-2xl shadow space-y-6">
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
          <div className="space-y-2 border-b pb-4">
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
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition"
            />

            <button
              onClick={handleProfileUpdate}
              className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-lg font-semibold transition"
            >
              Save Display Name
            </button>
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
              className="w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 transition"
            />

            <button
              onClick={handlePasswordChange}
              className="w-full bg-blue-600 hover:bg-blue-700 text-white py-2 rounded-lg font-semibold transition"
            >
              Change Password
            </button>
          </div>
        </div>
      </div>

      <div className="mb-10 bg-white p-6 rounded-2xl shadow space-y-6">
        <h2 className="text-2xl font-semibold mb-4">Your Reviews</h2>

        {reviews.length === 0 ? (
          <p className="text-gray-600">You haven't written any reviews yet</p>
        ) : (
          <div className="grid gap-4 sm:grid-cols-1 md:grid-cols-2">
            {reviews.map((review) => (
              <Link
                to={`/reviews/${review.id}`}
                key={review.id}
                className="block border border-gray-200 rounded-xl p-4 hover:shadow-lg transition bg-white"
              >
                <h3 className="text-lg font-semibold mb-1 truncate">{review.title}</h3>
                <p className="text-gray-600 text-sm line-clamp-3">Content: {review.content}</p>
                <p className="mt-2 flex items-center justify-between text-sm text-gray-500">
                  <span className="text-yellow-500 font-medium">
                    Rating: {review.rating}/5
                  </span>
                  <span>
                    {new Date(review.createdAt).toLocaleDateString()}
                  </span>
                </p>
              </Link>
            ))}
          </div>
        )}
      </div>

      <div className="mb-10 bg-white p-6 rounded-2xl shadow space-y-6">
        <h2 className="text-2xl font-semibold mb-4">Your Comments</h2>

        {comments.length === 0 ? (
          <p className="text-gray-600">You haven't written any comments yet</p>
        ) : (
          <div className="space-y-4">
            {comments.map((comment) => (
              <Link
                to={`/reviews/${comment.reviewId}`}
                key={comment.id}
                className="block border border-gray-200 rounded-xl p-4 hover:shadow-md transition bg-white"
              >
                <p className="text-gray-800 text-sm line-clamp-3 mb-2">{comment.content}</p>
                <p className="text-xs text-gray-500">{new Date(comment.createdAt).toLocaleDateString()}</p>
              </Link>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default Profile;
