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
    <div className="max-w-4xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-6">Your Profile</h1>

      <div className="mb-8 bg-white shadow rounded-2xl p-6 space-y-3">
        <p><strong>Name:</strong> {user?.displayName}</p>
        <p><strong>Email:</strong> {user?.email ?? "Not available"}</p>
      </div>

      <div className="mb-8 bg-white p-6 rounded-2xl shadow space-y-3">
        <h2 className="text-xl font-semibold">Edit Profile</h2>

        {successMessage && <p className="text-green-600">{successMessage}</p>}
        {errorMessages.length > 0 && (
          <ul className="mb-4 text-red-500 text-sm list-disc list-inside">
              {errorMessages.map((msg, i) => (
              <li key={i}>{msg}</li>
              ))}
          </ul>
        )}

        <div className="space-y-3">
            <label className="block">Display Name</label>
            <input
                value={newDisplayName}
                onChange={(e) => {
                  setNewDisplayName(e.target.value)
                  setSuccessMessage("");
                }}
                className="w-full border rounded-lg px-4 py-2"
            />
            <button
                onClick={handleProfileUpdate}
                className="mt-2 bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700"
            >
                Save Display Name
            </button>
        </div>
        
        <div className="space-y-3">
            <label className="block mb-1">Current Password</label>
            <input
                type="password"
                value={currentPassword}
                onChange={(e) => {
                  setCurrentPassword(e.target.value)
                  setSuccessMessage("");
                }}
                className="w-full border rounded-lg px-4 py-2"
            />
            <label className="block mt-4 mb-1">New Password</label>
            <input
                type="password"
                value={newPassword}
                onChange={(e) => {
                  setNewPassword(e.target.value)
                  setSuccessMessage("");
                }}
                className="w-full border rounded-lg px-4 py-2"
            />
            <button
                onClick={handlePasswordChange}
                className="mt-2 bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700"
            >
                Change Password
            </button>
        </div>
      </div>

      <div className="mb-8 bg-white p-6 rounded-2xl shadow space-y-6">
        <h2 className="text-2xl font-semibold mb-4">Your Reviews</h2>
        {reviews.length === 0 ? (
          <p className="text-gray-600">You haven't written any reviews yet</p>
        ) : (
          <ul className="space-y-4">
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
      </div>

      <div className="mb-8 bg-white p-6 rounded-2xl shadow space-y-6">
        <h2 className="text-2xl font-semibold mb-4">Your Comments</h2>
        {comments.length === 0 ? (
          <p className="text-gray-600">You haven't written any comments yet</p>
        ) : (
          <ul className="space-y-4">
            {comments.map((comment) => (
              <li key={comment.id}>
                <Link
                  to={`/reviews/${comment.reviewId}`}
                  className="block p-4 bg-white rounded shadow hover:shadow-md transition"
                >
                  <p className="text-gray-800">{comment.content}</p>
                  <p className="text-sm text-gray-500">{new Date(comment.createdAt).toLocaleDateString()}</p>
                </Link>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
};

export default Profile;
