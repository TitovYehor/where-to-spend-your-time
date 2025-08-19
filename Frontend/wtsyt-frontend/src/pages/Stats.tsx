import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import type { Stats } from '../types/stats';
import { getStats } from '../services/statsService';
import { handleApiError } from '../utils/handleApi';
import ReviewCard from '../components/reviews/ReviewCard';
import ItemStatCard from '../components/items/ItemStatCard';

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

  if (loading) return <p className="text-center mt-10 text-gray-500">Loading...</p>;
  if (!stats) return <p className="text-center mt-10 text-gray-500">No statistics available</p>;

  return (
    <div className="max-w-5xl mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-8 text-center">Platform Statistics</h1>

      <section className="mb-10 bg-white p-6 rounded-2xl shadow">
        <h2 className="text-2xl font-semibold mb-4">Top Rated Items</h2>

        {stats.topRatedItems.length === 0 ? (
          <p className="text-gray-500 italic">No top rated items yet</p>
        ) : (
          <div className="grid sm:grid-cols-1 md:grid-cols-2 gap-4">
            {stats.topRatedItems.map((item) => (
              <ItemStatCard key={item.id} item={item} />
            ))}
          </div>
        )}
      </section>

      <section className="mb-10 bg-white p-6 rounded-2xl shadow">
        <h2 className="text-2xl font-semibold mb-4">Most Reviewed Items</h2>
        {stats.mostReviewedItems.length === 0 ? (
          <p className="text-gray-500 italic">No most reviewed items yet</p>
        ) : (
          <div className="grid sm:grid-cols-1 md:grid-cols-2 gap-4">
            {stats.mostReviewedItems.map((item) => (
              <ItemStatCard key={item.id} item={item} />
            ))}
          </div>
        )}
      </section>

      <section className="mb-10 bg-white p-6 rounded-2xl shadow">
        <h2 className="text-2xl font-semibold mb-4">Top Reviewers</h2>
        {stats.topReviewers.length === 0 ? (
          <p className="text-gray-500 italic">No top reviewers yet</p>
        ) : (
          <div className="grid sm:grid-cols-1 md:grid-cols-2 gap-4">
            {stats.topReviewers.map((user) => (
                <Link 
                  to={`/users/${user.userId}`}
                  key={user.userId}
                  className="block border border-gray-100 rounded-xl p-4 bg-white shadow-sm hover:shadow-md transition"
                >
                  <h3 className="text-lg font-semibold">
                    {user.displayName}
                  </h3>
                  <p className="text-sm text-gray-500">{user.reviewCount} reviews</p>
                </Link>
            ))}
          </div>
        )}
      </section>

      <section className="mb-10 bg-white p-6 rounded-2xl shadow">
        <h2 className="text-2xl font-semibold mb-4">Recent Reviews</h2>
        {stats.recentReviews.length === 0 ? (
          <p className="text-gray-500 italic">No recent reviews yet</p>
        ) : (
          <div className="grid sm:grid-cols-1 md:grid-cols-2 gap-4">
            {stats.recentReviews.map((review) => (
              <ReviewCard key={review.id} review={review} />
            ))}
          </div>
        )}
      </section>
    </div>
  );
}