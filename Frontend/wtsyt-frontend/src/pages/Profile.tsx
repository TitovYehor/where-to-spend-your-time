import { useEffect, useState } from "react";
import { useAuth } from "../contexts/AuthContext";

type ReviewDto = {
  id: string;
  title: string;
  content: string;
  rating: number;
  createdAt: string;
};

type CommentDto = {
  id: string;
  content: string;
  createdAt: string;
};

const Profile = () => {
  const { user } = useAuth();
  const [loading, setLoading] = useState(true);
  const [reviews, setReviews] = useState<ReviewDto[]>([]);
  const [comments, setComments] = useState<CommentDto[]>([]);

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const response = await fetch("https://localhost:7005/api/users/me", {
          credentials: "include",
        });

        if (response.ok) {
          const data = await response.json();
          setReviews(data.reviews);
          setComments(data.comments);
        }
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

  return (
    <div className="max-w-4xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-6">Your Profile</h1>

      <div className="mb-8 bg-white shadow rounded-2xl p-6">
        <p><strong>Name:</strong> {user?.displayName}</p>
        <p><strong>Email:</strong> {user?.email ?? "Not available"}</p>
      </div>

      <div className="mb-8">
        <h2 className="text-2xl font-semibold mb-4">Your Reviews</h2>
        {reviews.length === 0 ? (
          <p className="text-gray-600">You haven't written any reviews yet</p>
        ) : (
          <ul className="space-y-4">
            {reviews.map((review) => (
              <li key={review.id} className="bg-white p-4 rounded-xl shadow">
                <h3 className="font-bold text-lg">{review.title}</h3>
                <p className="text-gray-700">{review.content}</p>
                <p className="text-sm text-gray-500">Rating: {review.rating} • {new Date(review.createdAt).toLocaleString()}</p>
              </li>
            ))}
          </ul>
        )}
      </div>

      <div>
        <h2 className="text-2xl font-semibold mb-4">Your Comments</h2>
        {comments.length === 0 ? (
          <p className="text-gray-600">You haven't written any comments yet</p>
        ) : (
          <ul className="space-y-4">
            {comments.map((comment) => (
              <li key={comment.id} className="bg-white p-4 rounded-xl shadow">
                <p className="text-gray-800">{comment.content}</p>
                <p className="text-sm text-gray-500">{new Date(comment.createdAt).toLocaleString()}</p>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  );
};

export default Profile;
