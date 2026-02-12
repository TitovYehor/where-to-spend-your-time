import { useEffect, useState } from 'react';
import type { Tag } from '../types/tag';
import { getTags } from '../services/tagService';
import { handleApiError } from '../utils/handleApi';
import { Tags as TagsIcon, Search as SearchIcon } from "lucide-react";
import TagCard from '../components/tags/TagCard';
import Alert from '../components/common/Alerts';
import { extractProblemDetailsError } from '../utils/extractProblemDetailsError';

export default function Tags() {
  const [tags, setTags] = useState<Tag[]>([]);
  const [search, setSearch] = useState("");

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | string[]>("");

  useEffect(() => {
    const controller = new AbortController();

    const fetchTags = async () => {
      setLoading(true);
      setError("");

      try {
        const result = await getTags(controller.signal);
        setTags(result);
      } catch (err: any) {
        if (!controller.signal.aborted) {
          handleApiError(err);
          setError(extractProblemDetailsError(err));
        }
      } finally {
        if (!controller.signal.aborted) {
          setLoading(false);
        }
      }
    };

    fetchTags();

    return () => controller.abort();
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
        <TagsIcon className="w-6 h-6 text-green-500" />
        Tags
      </h1>

      {loading ? (
        <p className="text-center mt-10">Loading...</p>
      ) : error ? (
        <Alert type="error" message={error} onClose={() => setError("")} />
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
                <TagCard key={t.id} tag={t} />
              ))}
            </div>
          )}
        </>
      )}
    </section>
  );
}