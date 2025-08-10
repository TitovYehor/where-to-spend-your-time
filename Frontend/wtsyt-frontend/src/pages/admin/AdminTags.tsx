import { useEffect, useRef, useState } from "react";
import { getTags, addTag, updateTag, deleteTag } from "../../services/tagService.ts";
import type { Tag } from "../../types/tag";
import { handleApiError } from "../../utils/handleApi";

export default function AdminTags() {
  const [tags, setTags] = useState<Tag[]>([]);

  const [loading, setLoading] = useState(true);

  const [name, setName] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);
  const [search, setSearch] = useState("");

  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  const formRef = useRef<HTMLFormElement>(null);

  const fetchTags = async () => {
    try {
      const data = await getTags();
      setTags(data);
    } catch (err) {
      handleApiError(err);
      setError("Failed to load tags");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTags();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (name.trim().length < 2) {
      setError("Tag name must be at least 2 characters");
      return;
    }

    try {
      if (editingId !== null) {
        await updateTag(editingId, { name });
        setEditingId(null);
        setMessage("Tag updated");
      } else {
        await addTag({ name });
        setMessage("Tag added");
      }
      setName("");
      fetchTags();
      setError("");
    } catch (err) {
      handleApiError(err);
      setError("Failed to save tag");
      setMessage("");
    }
  };

  const handleEdit = (tag: Tag) => {
    setEditingId(tag.id);
    setName(tag.name);
    
    setTimeout(() => {
        formRef.current?.scrollIntoView({ behavior: "smooth", block: "center" });
    }, 0);
    
    setError("");
    setMessage("");
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Are you sure you want to delete this tag?")) return;

    try {
      await deleteTag(id);
      fetchTags();
      setError("");
      setMessage("Tag deleted");
    } catch (err) {
      handleApiError(err);
      setError("Failed to delete tag");
      setMessage("");
    }
  };

  const filteredtags = tags.filter(t =>
    t.name.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <section
      aria-labelledby="manage-tags-heading"
      className="max-w-3xl mx-auto p-6 bg-white shadow rounded-xl"
    >
      <h1 id="manage-tags-heading" className="text-2xl font-bold mb-6">
        Manage Tags
      </h1>

      {message && (
        <div className="flex items-center justify-between bg-green-50 border border-green-300 text-green-800 text-sm px-4 py-2 rounded-md shadow-sm mb-3">
          <div>
            <span>{message}</span>
          </div>
          <button onClick={() => setMessage("")} className="text-green-600 hover:text-green-800">
            ✕
          </button>
        </div>
      )}

      {error && (
        <div className="flex items-center justify-between bg-red-50 border border-red-300 text-red-800 text-sm px-4 py-2 rounded-md shadow-sm mb-3">
          <div>
            <span>{error}</span>
          </div>
          <button onClick={() => setError("")} className="text-red-600 hover:text-red-800">
            ✕
          </button>
        </div>
      )}

      <form ref={formRef} onSubmit={handleSubmit} className="mb-6 space-y-4">
        <div>
          <label htmlFor="tagName" className="block text-sm font-medium text-gray-700 mb-1">
            Tag name
          </label>
          <input
            id="tagName"
            type="text"
            placeholder="Tag Name"
            className="w-full px-4 py-2 border rounded"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>
        <div className="flex items-center gap-4">
          <button
            type="submit"
            className="bg-blue-600 hover:bg-blue-700 text-white py-2 px-4 rounded transition"
          >
            {editingId ? "Update Tag" : "Add Tag"}
          </button>

          {editingId && (
            <button
              type="button"
              className="text-gray-500 text-sm underline hover:text-gray-700 transition"
              onClick={() => {
                setEditingId(null);
                setName("");
                setError("");
              }}
            >
              Cancel
            </button>
          )}
        </div>
      </form>

      <div className="mb-3">
        <label htmlFor="search" className="block text-sm font-medium text-gray-700 mb-1">
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

      {loading ? (
        <p>Loading...</p>
      ) : tags.length === 0 ? (
        <p className="text-gray-600">No tags found</p>
      ) : (
        <ul className="space-y-6">
          {filteredtags.map((tag) => (
            <li
              key={tag.id}
              className="flex flex-col sm:flex-row justify-between items-center bg-gray-50 border rounded-xl p-4 shadow-sm"
            >
              <span className="text-lg font-medium text-gray-900">{tag.name}</span>

              <div className="mt-3 sm:mt-0 sm:ml-6 flex flex-col gap-2 items-center">
                <button
                  onClick={() => handleEdit(tag)}
                  className="text-blue-600 hover:underline font-medium"
                  aria-label={`Edit ${tag.name}`}
                >
                  Edit
                </button>
                <button
                  onClick={() => handleDelete(tag.id)}
                  className="text-red-600 hover:underline font-medium"
                  aria-label={`Delete ${tag.name}`}
                >
                  Delete
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}