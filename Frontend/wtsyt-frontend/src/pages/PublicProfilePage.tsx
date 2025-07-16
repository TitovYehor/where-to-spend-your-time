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
    return <p className="text-center mt-8 text-red-500">User not found.</p>;
  }

  return (
    <div className="max-w-4xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-6">{user?.displayName}'s Profile</h1>

      <div className="mb-8">
        <h2 className="text-2xl font-semibold mb-4">User Reviews</h2>
        {user?.reviews.length === 0 ? (
          <p className="text-gray-600">User haven't written any reviews yet</p>
        ) : (
          <ul className="space-y-4">
            {user.reviews.map((review) => (
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

      <div>
        <h2 className="text-2xl font-semibold mb-4">User Comments</h2>
        {user?.comments.length === 0 ? (
          <p className="text-gray-600">User haven't written any comments yet</p>
        ) : (
          <ul className="space-y-4">
            {user.comments.map((comment) => (
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