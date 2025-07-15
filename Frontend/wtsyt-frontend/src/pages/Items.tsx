import { useEffect, useState } from 'react';
import type { ItemsResult } from '../types/item.ts';
import { getItems } from '../services/itemService.ts';
import { Link } from 'react-router-dom';
import { handleApiError } from '../utils/handleApi.ts';

export default function Items() {
  const [itemsResult, setItems] = useState<ItemsResult>();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchItems = async () => {
      setLoading(true);
      setError("");

      try {
        const result = await getItems({});
        setItems(result);
      } catch (e) {
        handleApiError(e);
        setError("Failed to load items.");
      } finally {
        setLoading(false);
      }
    };

    fetchItems();
  }, []);

  if (loading) return <p className="text-center mt-10">Loading...</p>;
  if (error) return <p className="text-center text-red-500 mt-10">{error}</p>;
  if (!itemsResult || itemsResult.totalCount === 0) {
    return <p className="text-center mt-10 text-gray-600">No items found.</p>;
  }

  return (
    <div className="max-w-4xl mx-auto px-4">
      <h1 className="text-2xl font-bold mb-4">Items</h1>
      
      <div className="mb-4 text-gray-700">
        {itemsResult?.totalCount} item{itemsResult?.totalCount !== 1 ? "s" : ""} found
      </div>
      <div className="grid gap-4">
        {itemsResult?.items.map(item => (
          <Link
            key={item.id}
            to={`/items/${item.id}`}
            className="block bg-white rounded-xl shadow p-4 hover:shadow-md transition"
          >
            <h3 className="text-lg font-semibold">{item.title}</h3>
            <p className="text-sm text-gray-600">Category: {item.categoryName}</p>
            <p className="text-gray-800">Description: {item.description}</p>
            <p className="text-yellow-500">Rating: {item.averageRating}/5</p>
          </Link>
        ))}
      </div>
    </div>
  );
}