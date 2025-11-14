import { useEffect, useState } from 'react';
import type { Category } from '../types/category';
import { getCategories } from '../services/categoryService';
import { handleApiError } from '../utils/handleApi';
import { Search, Folders } from "lucide-react";
import CategoryCard from '../components/categories/CategoryCard';
import Alert from '../components/common/Alerts';

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
        const message = handleApiError(e);
        setError(message);
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
      className="max-w-4xl mx-auto p-8 bg-white/80 backdrop-blur-md rounded-2xl shadow-xl"
    >
      <h1 id="categories-heading" className="text-2xl font-bold mb-4 flex items-center gap-2">
        <Folders className="w-6 h-6 text-blue-500" />
        Categories
      </h1>

      {loading ? (
        <p className="text-center mt-10">Loading...</p>
      ) : error ? (
        <Alert type="error" message={error} onClose={() => setError("")} />
      ) : (
        <>
          <div>
            <label htmlFor="search" className="block text-sm font-medium text-black mb-1 flex items-center gap-1">
              <Search className="w-4 h-4 text-gray-500" />
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
                <CategoryCard key={cat.id} category={cat} />
              ))}
            </div>
          )}
        </>
      )}
    </section>
  );
}