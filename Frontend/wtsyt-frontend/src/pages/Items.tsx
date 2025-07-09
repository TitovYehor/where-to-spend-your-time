import { useEffect, useState } from 'react';
import { getItems, type ItemsResult } from '../api/itemService.ts';
import { Link } from 'react-router-dom';

export default function Items() {
  const [itemsResult, setItems] = useState<ItemsResult>();
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getItems()
      .then(setItems)
      .catch((e) => console.error('Failed to fetch items', e))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <p className="text-center mt-10">Loading...</p>;

  return (
    <div className="max-w-4xl mx-auto px-4">
      <h1 className="text-2xl font-bold mb-4">Items</h1>
      
      <div className="mb-4 text-gray-700">
        {itemsResult?.totalCount} item{itemsResult?.totalCount !== 1 ? "s" : ""} found
      </div>
      <br/>
      <div className="grid gap-4">
        {itemsResult?.items.map(item => (
          <div
            key={item.id}
            className="bg-white rounded-xl shadow p-4 hover:shadow-md transition"
          >
            <Link to={`/items/${item.id}`}>
              {item.title}
            </Link>
            <p className="text-gray-600">{item.description}</p>
            <div className="text-sm text-gray-500 mt-1">
              Category: <span className="font-medium">{item.categoryName}</span> · Rating: <span className="font-medium">{item.averageRating.toFixed(1)}</span>
            </div>
            <br />
          </div>
        ))}
      </div>
    </div>
  );
}