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
    <section
      aria-labelledby="categories-heading"
      className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8 bg-white/60 backdrop-blur-md rounded-xl shadow-lg"
    >
      <h1 id="categories-heading" className="text-2xl font-bold mb-4">
        Categories
      </h1>

      {loading ? (
        <p className="text-center mt-10">Loading...</p>
      ) : error ? (
        <p className="text-center text-red-500 mt-10">{error}</p>
      ) : (
        <>
          <div>
            <label htmlFor="search" className="block text-sm font-medium text-black mb-1">
              Search
            </label>
            <input
              id="search"
              type="text"
              placeholder="Search categories..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-full border border-gray-300 px-4 py-2 rounded-lg shadow-sm mb-6 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {filteredCategories.length === 0 ? (
            <p className="text-center mt-10 text-gray-500">No categories match your search</p>
          ) : (
            <div className="grid gap-4 sm:grid-cols-2 md:grid-cols-3">
              {filteredCategories.map(cat => (
                <Link
                  key={cat.id}
                  to={`/?categoryId=${cat.id}`}
                  className="bg-white rounded-xl shadow p-4 hover:shadow-md transition-transform hover:scale-[1.02] focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <h3 className="text-lg font-semibold text-gray-800">{cat.name}</h3>
                </Link>
              ))}
            </div>
          )}
        </>
      )}
    </section>
  );
}