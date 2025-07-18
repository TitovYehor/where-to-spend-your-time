import { useEffect, useRef, useState } from "react";
import { getCategories, addCategory, updateCategory, deleteCategory } from "../../services/categoryService";
import type { Category } from "../../types/category";
import { handleApiError } from "../../utils/handleApi";

export default function AdminCategories() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [name, setName] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);
  const [error, setError] = useState("");

  const formRef = useRef<HTMLFormElement>(null);

  const fetchCategories = async () => {
    try {
      const data = await getCategories();
      setCategories(data);
    } catch (err) {
      handleApiError(err);
      setError("Failed to load categories");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (name.trim().length < 2) {
      setError("Category name must be at least 2 characters.");
      return;
    }

    try {
      if (editingId !== null) {
        await updateCategory(editingId, { name });
        setEditingId(null);
      } else {
        await addCategory({ name });
      }
      setName("");
      fetchCategories();
    } catch (err) {
      handleApiError(err);
      setError("Failed to save category.");
    }
  };

  const handleEdit = (category: Category) => {
    setEditingId(category.id);
    setName(category.name);
    
    setTimeout(() => {
        formRef.current?.scrollIntoView({ behavior: "smooth", block: "center" });
    }, 0);
    
    setError("");
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Are you sure you want to delete this category?")) return;

    try {
      await deleteCategory(id);
      fetchCategories();
    } catch (err) {
      handleApiError(err);
      setError("Failed to delete category.");
    }
  };

  return (
    <div className="max-w-3xl mx-auto p-6 bg-white shadow rounded-xl">
      <h1 className="text-2xl font-bold mb-6">Manage Categories</h1>

      <form ref={formRef} onSubmit={handleSubmit} className="mb-6 space-y-4">
        <div>
          <input
            type="text"
            placeholder="Category Name"
            className="w-full px-4 py-2 border rounded"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>
        <button
          type="submit"
          className="bg-blue-600 hover:bg-blue-700 text-white py-2 px-4 rounded"
        >
          {editingId ? "Update Category" : "Add Category"}
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
      ) : categories.length === 0 ? (
        <p className="text-gray-600">No categories found.</p>
      ) : (
        <ul className="space-y-4">
          {categories.map((cat) => (
            <li
              key={cat.id}
              className="flex justify-between items-center border p-4 rounded"
            >
              <span>{cat.name}</span>
              <div className="space-x-2">
                <button
                  onClick={() => handleEdit(cat)}
                  className="text-blue-600 hover:underline"
                >
                  Edit
                </button>
                <button
                  onClick={() => handleDelete(cat.id)}
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