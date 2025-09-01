import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import type { AuthUser } from "../types/authUser";
import { getProfileById } from "../services/userService";
import { handleApiError } from "../utils/handleApi";
import ReviewCard from "../components/reviews/ReviewCard";
import CommentCard from "../components/comments/CommentCard";

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
    <div className="max-w-4xl mx-auto p-8 bg-white/80 backdrop-blur-md rounded-2xl shadow-xl">
      <article className="mb-10">
        <h1 className="text-3xl font-bold mb-2">{user?.displayName}'s Profile</h1>
        <p><strong>Reviews count:</strong> {user?.reviews.length}</p>
        <p><strong>Comments count:</strong> {user?.comments.length}</p>
      </article>

      <div className="mb-12 space-y-6">
        <h2 className="text-2xl font-semibold">User Reviews</h2>
        {user?.reviews.length === 0 ? (
          <p className="text-gray-600">User haven't written any reviews yet</p>
        ) : (
          <div className="grid gap-4 sm:grid-cols-1 md:grid-cols-2">
            {user.reviews.map((review) => (
              <ReviewCard key={review.id} review={review} />
            ))}
          </div>
        )}
      </div>

      <div className="mb-10  space-y-6">
        <h2 className="text-2xl font-semibold mb-4">User Comments</h2>
        {user?.comments.length === 0 ? (
          <p className="text-gray-600">User haven't written any comments yet</p>
        ) : (
          <div className="space-y-4">
            {user.comments.map((comment) => (
              <CommentCard key={comment.id} comment={comment} />
            ))}
          </div>
        )}
      </div>
    </div>
  );
};