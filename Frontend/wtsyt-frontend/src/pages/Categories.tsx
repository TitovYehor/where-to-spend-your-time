import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import type { Category } from '../types/category';
import { getCategories } from '../services/categoryService';

export default function Categories() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getCategories()
      .then(setCategories)
      .catch((e) => console.error('Failed to fetch items', e))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <p className="text-center mt-10">Loading...</p>;

  return (
    <div className="max-w-4xl mx-auto px-4">
      <h1 className="text-2xl font-bold mb-4">Categories</h1>
      <div className="grid gap-4">
        {categories.map(cat => (
          <Link
            key={cat.id}
            to={`/categories/${cat.id}`}
            className="bg-white rounded-xl shadow p-4 hover:shadow-md transition"
          >
            <h3 className="text-lg font-semibold">{cat.name}</h3>
          </Link>
        ))}
      </div>
    </div>
  );
}