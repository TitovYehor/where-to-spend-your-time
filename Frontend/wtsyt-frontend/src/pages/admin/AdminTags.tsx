import { useEffect, useRef, useState } from "react";
import { getTags, addTag, updateTag, deleteTag } from "../../services/tagService.ts";
import type { Tag } from "../../types/tag";
import { handleApiError } from "../../utils/handleApi";

export default function AdminTags() {
  const [tags, setTags] = useState<Tag[]>([]);
  const [loading, setLoading] = useState(true);
  const [name, setName] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);
  const [error, setError] = useState("");

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
      } else {
        await addTag({ name });
      }
      setName("");
      fetchTags();
    } catch (err) {
      handleApiError(err);
      setError("Failed to save tag");
    }
  };

  const handleEdit = (tag: Tag) => {
    setEditingId(tag.id);
    setName(tag.name);
    
    setTimeout(() => {
        formRef.current?.scrollIntoView({ behavior: "smooth", block: "center" });
    }, 0);
    
    setError("");
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Are you sure you want to delete this tag?")) return;

    try {
      await deleteTag(id);
      fetchTags();
    } catch (err) {
      handleApiError(err);
      setError("Failed to delete tag");
    }
  };

  return (
    <div className="max-w-3xl mx-auto p-6 bg-white shadow rounded-xl">
      <h1 className="text-2xl font-bold mb-6">Manage Tags</h1>

      <form ref={formRef} onSubmit={handleSubmit} className="mb-6 space-y-4">
        <div>
          <input
            type="text"
            placeholder="Tag Name"
            className="w-full px-4 py-2 border rounded"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>
        <button
          type="submit"
          className="bg-blue-600 hover:bg-blue-700 text-white py-2 px-4 rounded"
        >
          {editingId ? "Update Tag" : "Add Tag"}
        </button>
        {editingId && (
          <button
            type="button"
            className="ml-4 text-sm text-gray-500 underline"
            onClick={() => {
              setEditingId(null);
              setName("");
              setError("");
            }}
          >
            Cancel
          </button>
        )}
        {error && <p className="text-red-500 text-sm">{error}</p>}
      </form>

      {loading ? (
        <p>Loading...</p>
      ) : tags.length === 0 ? (
        <p className="text-gray-600">No tags found</p>
      ) : (
        <ul className="space-y-4">
          {tags.map((tag) => (
            <li
              key={tag.id}
              className="flex justify-between items-center border p-4 rounded"
            >
              <span>{tag.name}</span>
              <div className="space-x-2">
                <button
                  onClick={() => handleEdit(tag)}
                  className="text-blue-600 hover:underline"
                >
                  Edit
                </button>
                <button
                  onClick={() => handleDelete(tag.id)}
                  className="text-red-600 hover:underline"
                >
                  Delete
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}