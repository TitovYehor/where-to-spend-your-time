import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import type { Category } from '../types/category';
import { getCategories } from '../services/categoryService';
import { handleApiError } from '../utils/handleApi';

export default function Categories() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [search, setSearch] = useState("");

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

  const filteredCategories = categories.filter(cat =>
    cat.name.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="max-w-4xl mx-auto px-4">
      <h1 className="text-2xl font-bold mb-4">Categories</h1>

      {loading ? (
        <p className="text-center mt-10">Loading...</p>
      ) : error ? (
        <p className="text-center text-red-500 mt-10">{error}</p>
      ) : (
        <>
          <input
            type="text"
            placeholder="Search categories..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-full border px-3 py-2 rounded mb-6"
          />

          {filteredCategories.length === 0 ? (
            <p className="text-center mt-10 text-gray-500">No categories match your search</p>
          ) : (
            <div className="grid gap-4">
              {filteredCategories.map(cat => (
                <Link
                  key={cat.id}
                  to={`/?categoryId=${cat.id}`}
                  className="bg-white rounded-xl shadow p-4 hover:shadow-md transition"
                >
                  <h3 className="text-lg font-semibold">{cat.name}</h3>
                </Link>
              ))}
            </div>
          )}
        </>
      )}
    </div>
  );
}