import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

type ItemStat = {
  id: number;
  title: string;
  category: string;
  averageRating: number;
  reviewCount: number;
};

type UserStat = {
    userId: string;
    displayName: string;
    reviewCount: number;
};

type Review = {
  id: number;
  title: string;
  content: string;
  rating: number;
  createdAt: string;
  author: string;
};

type Stats = {
  topRatedItems: ItemStat[];
  mostReviewedItems: ItemStat[];
  topReviewers: UserStat[];
  recentReviews: Review[];
};

export default function Stats() {
  const [stats, setStats] = useState<Stats | null>();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`https://localhost:7005/api/stats`, {
          credentials: "include",
        });

        if (res.ok) {
          const data = await res.json();
          setStats(data);
        } else {
          console.error("Failed to fetch stats");
        }
      } catch (err) {
        console.error("Fetch error:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) return <p className="text-center mt-10">Loading...</p>;
  if (!stats) return <p className="text-center mt-10">No statistic formed</p>;

  return (
    <div className="max-w-4xl mx-auto px-4 py-6">
      <h1 className="text-2xl font-bold mb-4">Platform Statistics</h1>

      <section className="mb-8">
        <h2 className="text-xl font-semibold mb-2">Top Rated Items</h2>
        <ul>
          {stats.topRatedItems.map((item) => (
            <li key={item.id}>
              {item.title} - {item.averageRating}/5 ({item.reviewCount} reviews)
              <Link to={`/items/${item.id}`}>
                Go to '{item.title}' details
              </Link> 
            </li>
          ))}
        </ul>
      </section>

      <section className="mb-8">
        <h2 className="text-xl font-semibold mb-2">Most Reviewed Items</h2>
        <ul>
          {stats.mostReviewedItems.map((item) => (
            <li key={item.id}>
              {item.title} - {item.reviewCount} reviews
              <Link to={`/items/${item.id}`}>
                Go to '{item.title}' details
              </Link>
            </li>
          ))}
        </ul>
      </section>

      <section className="mb-8">
        <h2 className="text-xl font-semibold mb-2">Top Reviewers</h2>
        <ul>
          {stats.topReviewers.map((user) => (
            <li key={user.userId}>
              {user.displayName} - {user.reviewCount} reviews
            </li>
          ))}
        </ul>
      </section>

      <section>
        <h2 className="text-xl font-semibold mb-2">Recent Reviews</h2>
        <ul>
          {stats.recentReviews.map((review) => (
            <li key={review.id}>
              <strong>{review.title}</strong> by {review.author} -{" "}
              {review.rating}/5
              <p>{review.content}</p>
              <p className="text-sm text-gray-500">
                {new Date(review.createdAt).toLocaleString()}
              </p>
              <Link to={`/reviews/${review.id}`}>
                Go to '{review.title}' details
              </Link>
            </li>
          ))}
        </ul>
      </section>
    </div>
  );
}