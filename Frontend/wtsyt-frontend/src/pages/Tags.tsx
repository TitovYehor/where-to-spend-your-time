import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import type { Tag } from '../types/tag';
import { getTags } from '../services/tagService';
import { handleApiError } from '../utils/handleApi';
import { Tags as TagsIcon, Search as SearchIcon, Tag as TagIcon } from "lucide-react";

export default function Tags() {
  const [tags, setTags] = useState<Tag[]>([]);
  const [search, setSearch] = useState("");

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const fetchTags = async () => {
      setLoading(true);
      setError("");

      try {
        const result = await getTags();
        setTags(result);
      } catch (e) {
        handleApiError(e);
        setError("Failed to load tags");
      } finally {
        setLoading(false);
      }
    };

    fetchTags();
  }, []);

  const filteredTags = tags.filter(t =>
    t.name.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <section
      aria-labelledby="tags-heading"
      className="max-w-4xl mx-auto p-8 bg-white/80 backdrop-blur-md rounded-2xl shadow-xl"
    >
      <h1 id="tags-heading" className="text-2xl font-bold mb-4 flex items-center gap-2">
        <TagsIcon className="w-6 h-6 text-blue-600" />
        Tags
      </h1>

      {loading ? (
        <p className="text-center mt-10">Loading...</p>
      ) : error ? (
        <p className="text-center text-red-500 mt-10">{error}</p>
      ) : (
        <>
          <div>
            <label htmlFor="search" className="block text-sm font-medium text-black mb-1 flex items-center gap-1">
              <SearchIcon className="w-4 h-4 text-gray-500" />
              Search
            </label>
            <input
              id="search"
              type="text"
              placeholder="Search tags..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="w-full border border-gray-300 px-4 py-2 rounded-lg shadow-sm mb-6 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
          </div>

          {filteredTags.length === 0 ? (
            <p className="text-center mt-10 text-gray-500">No tags match your search</p>
          ) : (
            <div className="grid gap-4 sm:grid-cols-2 md:grid-cols-3">
              {filteredTags.map(t => (
                <Link
                  key={t.id}
                  to={`/?tagsids=${t.id}`}
                  className="bg-white rounded-xl shadow p-4 hover:shadow-md transition-transform hover:scale-[1.02] focus:outline-none focus:ring-2 focus:ring-blue-500 flex items-center gap-2"
                >
                  <TagIcon className="w-5 h-5 text-green-500" />
                  <h3 className="text-lg font-semibold text-gray-800">{t.name}</h3>
                </Link>
              ))}
            </div>
          )}
        </>
      )}
    </section>
  );
}