import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import type { Stats } from '../types/stats';
import { getStats } from '../services/statsService';
import { handleApiError } from '../utils/handleApi';

export default function Stats() {
  const [stats, setStats] = useState<Stats | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const data = await getStats();
        setStats(data);
      } catch (e) {
        handleApiError(e);
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

      <section className="mb-8 bg-white p-6 rounded-2xl shadow space-y-4">
        <h2 className="text-xl font-semibold mb-2">Top Rated Items</h2>
        {stats.topRatedItems.length === 0 ? (
          <p className="text-gray-500">No top rated items yet.</p>
        ) : (
          <ul className="space-y-3">
            {stats.topRatedItems.map((item) => (
              <li key={item.id}>
                <Link
                  to={`/items/${item.id}`}
                  className="block p-4 bg-white rounded shadow hover:shadow-md transition"
                >
                  <h3 className="text-lg font-semibold">{item.title}</h3>
                  <p className="text-sm text-gray-600">Category: {item.category}</p>
                  <p className="text-yellow-500">Rating: {item.averageRating}/5</p>
                </Link>
              </li>
            ))}
          </ul>
        )}
      </section>

      <section className="mb-8 bg-white p-6 rounded-2xl shadow space-y-4">
        <h2 className="text-xl font-semibold mb-2">Most Reviewed Items</h2>
        {stats.mostReviewedItems.length === 0 ? (
          <p className="text-gray-500">No most reviewed items yet.</p>
        ) : (
          <ul className="space-y-3">
            {stats.mostReviewedItems.map((item) => (
              <li key={item.id}>
                <Link 
                  to={`/items/${item.id}`}
                  className="block p-4 bg-white rounded shadow hover:shadow-md transition"
                >
                  <h3 className="text-lg font-semibold">{item.title}</h3>
                  <p className="text-sm text-gray-600">Category: {item.category}</p>
                  <p className="text-yellow-500">Rating: {item.averageRating}/5</p>
                </Link>
              </li>
            ))}
          </ul>
        )}
      </section>

      <section className="mb-8 bg-white p-6 rounded-2xl shadow space-y-4">
        <h2 className="text-xl font-semibold mb-2">Top Reviewers</h2>
        {stats.topReviewers.length === 0 ? (
          <p className="text-gray-500">No top reviewers yet.</p>
        ) : (
          <ul className="space-y-3">
            {stats.topReviewers.map((user) => (
              <li key={user.userId}>
                <Link 
                  to={`/users/${user.userId}`}
                  className="block p-4 bg-white rounded shadow hover:shadow-md transition"
                >
                  <h2 className="text-lg font-semibold">{user.displayName} - {user.reviewCount} reviews</h2>
                </Link>
              </li>
            ))}
          </ul>
        )}
      </section>

      <section className="mb-8 bg-white p-6 rounded-2xl shadow space-y-4">
        <h2 className="text-xl font-semibold mb-2">Recent Reviews</h2>
        {stats.recentReviews.length === 0 ? (
          <p className="text-gray-500">No recent reviews yet.</p>
        ) : (
          <ul className="space-y-3">
            {stats.recentReviews.map((review) => (
              <li key={review.id}>
                <Link 
                  to={`/reviews/${review.id}`}
                  className="block p-4 bg-white rounded shadow hover:shadow-md transition"
                >
                  <h3 className="text-lg font-semibold">{review.title}</h3>
                  <p className="text-sm text-gray-600">By {review.author}</p>
                  <p className="text-yellow-500">Rating: {review.rating}/5</p>
                  <p className="text-sm text-gray-500">
                    {new Date(review.createdAt).toLocaleDateString()}
                  </p>
                </Link>
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  );
}