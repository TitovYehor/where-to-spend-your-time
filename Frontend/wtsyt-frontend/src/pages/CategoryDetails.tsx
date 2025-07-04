import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Link } from 'react-router-dom';

type Category = {
  id: number;
  name: string;
};

type Item = {
  id: number;
  title: string;
  description: string;
  categoryName: string;
  averageRating: number;
};

export default function CategoryDetails() {
  const { categoryId } = useParams<{ categoryId: string }>();

  const [category, setCategory] = useState<Category | null>(null);
  const [items, setItems] = useState<Item[]>([]);

  const fetchData = async () => {
    const [catRes, itemsRes] = await Promise.all([
      fetch(`https://localhost:7005/api/categories/${categoryId}`),
      fetch(`https://localhost:7005/api/categories/${categoryId}/items`),
    ]);

    if (catRes.ok) {
      const catData = await catRes.json();
      setCategory(catData);
    }

    if (itemsRes.ok) {
      const itemsData = await itemsRes.json();
      setItems(itemsData);
    }
  };

  useEffect(() => {
    fetchData();
  }, [categoryId]);

  if (!category) return <div className="p-6">Loading category...</div>;

  return (
    <div className="max-w-3xl mx-auto px-4 py-6">
      <h1 className="text-3xl font-bold mb-2">'{category.name}' category</h1>

      <h2 className="text-xl font-semibold mb-4">Items</h2>
      {items.length === 0 ? (
        <p className="text-gray-500 mb-6">No items yet</p>
      ) : (
        <ul className="space-y-4 mb-6">
          {items.map((i) => (
            <li key={i.id} className="bg-white p-4 rounded shadow">
              <div className="flex justify-between mb-1">
                <strong>{i.title}</strong>
                <span className="text-sm text-gray-500">Rating: {i.averageRating}/5</span>
              </div>
              <div className="font-medium text-yellow-500">Description: {i.description}</div>
              <Link to={`/items/${i.id}`}>
                Go to '{i.title}' details page
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}