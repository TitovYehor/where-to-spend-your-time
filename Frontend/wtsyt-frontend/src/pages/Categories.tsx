import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import type { Category } from '../types/category';
import { getCategories } from '../services/categoryService';
import { handleApiError } from '../utils/handleApi';

export default function Categories() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchCategories = async () => {
      setLoading(true);
      setError("");

      try {
        const result = await getCategories();
        setCategories(result);
      } catch (e) {
        handleApiError(e);
        setError("Failed to load categories.");
      } finally {
        setLoading(false);
      }
    };

    fetchCategories();
  }, []);

  if (loading) return <p className="text-center mt-10">Loading...</p>;
  if (error) return <p className="text-center text-red-500 mt-10">{error}</p>;
  if (!loading && categories.length === 0) {
    return <p className="text-center mt-10 text-gray-500">No categories found.</p>;
  }

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