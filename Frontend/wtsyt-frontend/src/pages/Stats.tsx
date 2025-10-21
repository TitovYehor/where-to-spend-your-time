import { useEffect, useState } from 'react';
import type { Stats } from '../types/stats';
import { getStats } from '../services/statsService';
import { handleApiError } from '../utils/handleApi';
import ReviewCard from '../components/reviews/ReviewCard';
import ItemStatCard from '../components/items/ItemStatCard';
import UserProfileLink from '../components/users/UserProfileLinks';
import { BarChart3, Star, Boxes, FileText, Users, Clock } from "lucide-react";

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
    <div className="max-w-4xl mx-auto p-8 bg-white/80 backdrop-blur-md rounded-2xl shadow-xl">
      <h1 className="text-3xl font-bold mb-8 text-center flex items-center justify-center gap-2">
        <BarChart3 className="w-8 h-8 text-blue-600" />
        Platform Statistics
      </h1>

      <section className="mb-10">
        <h2 className="text-2xl font-semibold mb-4 flex items-center gap-2">
          <Boxes className="w-6 h-6 text-indigo-500" />
          <Star className="w-6 h-6 text-yellow-500" />
          Top Rated Items
        </h2>

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

      <section className="mb-10">
        <h2 className="text-2xl font-semibold mb-4 flex items-center gap-2">
          <Boxes className="w-6 h-6 text-indigo-500" />
          <FileText className="w-6 h-6 text-blue-500" />
          Most Reviewed Items
        </h2>

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

      <section className="mb-10">
        <h2 className="text-2xl font-semibold mb-4 flex items-center gap-2">
          <Users className="w-6 h-6 text-green-600" />
          Top Reviewers
        </h2>

        {stats.topReviewers.length === 0 ? (
          <p className="text-gray-500 italic">No top reviewers yet</p>
        ) : (
          <div className="grid sm:grid-cols-1 md:grid-cols-2 gap-4">
            {stats.topReviewers.map((reviewer) => (
              <div
                key={reviewer.userId}
                className="block border border-gray-100 rounded-xl p-4 bg-white shadow-sm hover:shadow-md transition"
              >
                <h3 className="text-lg font-semibold">
                  <UserProfileLink userId={reviewer.userId} name={reviewer.displayName} />
                </h3>
                <p className="text-sm text-gray-500">{reviewer.reviewCount} reviews</p>
              </div>
            ))}
          </div>
        )}
      </section>

      <section className="mb-10">
        <h2 className="text-2xl font-semibold mb-4 flex items-center gap-2">
          <Clock className="w-6 h-6 text-purple-600" />
          <FileText className="w-6 h-6 text-blue-500" />
          Recent Reviews
        </h2>
        
        {stats.recentReviews.length === 0 ? (
          <p className="text-gray-500 italic">No recent reviews yet</p>
        ) : (
          <div className="grid sm:grid-cols-1 md:grid-cols-2 gap-4 items-stretch">
            {stats.recentReviews.map((review) => (
              <ReviewCard key={review.id} review={review} />
            ))}
          </div>
        )}
      </section>
    </div>
  );
}