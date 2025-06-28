import { useEffect, useState } from 'react';
import { getItems, type ItemDto } from '../api/itemService.ts';

export default function Items() {
  const [items, setItems] = useState<ItemDto[]>([]);
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
      <div className="grid gap-4">
        {items.map(item => (
          <div
            key={item.id}
            className="bg-white rounded-xl shadow p-4 hover:shadow-md transition"
          >
            <h2 className="text-lg font-semibold">{item.title}</h2>
            <p className="text-gray-600">{item.description}</p>
            <div className="text-sm text-gray-500 mt-1">
              Category: <span className="font-medium">{item.categoryName}</span> · Rating: <span className="font-medium">{item.averageRating.toFixed(1)}</span>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}