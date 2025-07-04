import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

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

type ApplicationUser = {
    userId: string;
    displayName: string;

    reviews: ReviewDto[];
    comments: CommentDto[];
};

export default function PublicProfile() {
  const { userId } = useParams<{ userId: string }>();

  const [loading, setLoading] = useState(true);
  const [user, setUser] = useState<ApplicationUser | null>(null);

  useEffect(() => {
    const fetchProfile = async () => {
      try {
        const response = await fetch(`https://localhost:7005/api/users/${userId}`, {
          credentials: "include",
        });

        if (response.ok) {
          const data = await response.json();
          setUser(data);
        }
      } finally {
        setLoading(false);
      }
    };

    fetchProfile();
  }, [userId]);

  if (loading) {
    return <p className="text-center mt-8">Loading...</p>;
  }

  return (
    <div className="max-w-4xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-6">'{user?.displayName}' Profile</h1>

      <div className="mb-8">
        <h2 className="text-2xl font-semibold mb-4">User Reviews</h2>
        {user?.reviews.length === 0 ? (
          <p className="text-gray-600">User haven't written any reviews yet</p>
        ) : (
          <ul className="space-y-4">
            {user?.reviews.map((review) => (
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
        <h2 className="text-2xl font-semibold mb-4">User Comments</h2>
        {user?.comments.length === 0 ? (
          <p className="text-gray-600">User haven't written any comments yet</p>
        ) : (
          <ul className="space-y-4">
            {user?.comments.map((comment) => (
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