import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import type { AuthUser } from "../types/authUser";
import { getProfileById } from "../services/userService";
import { handleApiError } from "../utils/handleApi";

export default function PublicProfile() {
  const { userId } = useParams<{ userId: string }>();

  const [loading, setLoading] = useState(true);
  const [user, setUser] = useState<AuthUser | null>(null);

  useEffect(() => {
    const fetchProfile = async () => {
      if (!userId) return;

      try {
        const data = await getProfileById(userId);
        setUser(data);
      } catch (err) {
        handleApiError(err);
      } finally {
        setLoading(false);
      }
    };

    fetchProfile();
  }, [userId]);

  if (loading) {
    return <p className="text-center mt-8">Loading...</p>;
  }

  if (!user) {
    return <p className="text-center mt-8 text-red-500">User not found</p>;
  }

  return (
    <div className="max-w-4xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-8">{user?.displayName}'s Profile</h1>

      <div className="mb-12 bg-white p-6 rounded-2xl shadow space-y-6">
        <h2 className="text-2xl font-semibold">User Reviews</h2>
        {user?.reviews.length === 0 ? (
          <p className="text-gray-600">User haven't written any reviews yet</p>
        ) : (
          <div className="grid gap-4 sm:grid-cols-1 md:grid-cols-2">
            {user.reviews.map((review) => (
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
        <h2 className="text-2xl font-semibold mb-4">User Comments</h2>
        {user?.comments.length === 0 ? (
          <p className="text-gray-600">User haven't written any comments yet</p>
        ) : (
          <div className="space-y-4">
            {user.comments.map((comment) => (
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