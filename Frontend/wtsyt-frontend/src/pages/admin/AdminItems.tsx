import { useEffect, useRef, useState } from "react";
import { getItems, addItem, updateItem, deleteItem } from "../../services/itemService";
import { getCategories } from "../../services/categoryService";
import type { Item, ItemCreateRequest } from "../../types/item";
import type { Category } from "../../types/category";
import { handleApiError } from "../../utils/handleApi";

export default function AdminItems() {
  const [items, setItems] = useState<Item[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [form, setForm] = useState<ItemCreateRequest>({
    title: "",
    description: "",
    categoryId: 0,
  });
  const [editingId, setEditingId] = useState<number | null>(null);
  const [error, setError] = useState("");

  const formRef = useRef<HTMLFormElement>(null);

  const fetchItemsAndCategories = async () => {
    try {
      const [itemList, categoryList] = await Promise.all([
        getItems({}),
        getCategories(),
      ]);
      setItems(itemList.items);
      setCategories(categoryList);
    } catch (err) {
      handleApiError(err);
      setError("Failed to fetch data");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchItemsAndCategories();
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement>) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.title || !form.categoryId) {
      setError("Title and category are required");
      return;
    }

    try {
      if (editingId !== null) {
        await updateItem(editingId, form);
        setEditingId(null);
      } else {
        await addItem(form);
      }
      setForm({ title: "", description: "", categoryId: 0 });
      fetchItemsAndCategories();
    } catch (err) {
      handleApiError(err);
      setError("Failed to save item");
    }
  };

  const handleEdit = (item: Item) => {
    setEditingId(item.id);
    
    setForm({
      title: item.title,
      description: item.description || "",
      categoryId: item.categoryId,
    });

    setTimeout(() => {
        formRef.current?.scrollIntoView({ behavior: "smooth", block: "center" });
    }, 0);

    setError("");
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Are you sure you want to delete this item?")) return;

    try {
      await deleteItem(id);
      fetchItemsAndCategories();
    } catch {
      setError("Failed to delete item");
    }
  };

  return (
    <div className="max-w-4xl mx-auto p-6 bg-white rounded-xl shadow">
      <h1 className="text-2xl font-bold mb-6">Manage Items</h1>

      <form ref={formRef} onSubmit={handleSubmit} className="space-y-4 mb-8">
        <input
          name="title"
          type="text"
          placeholder="Title"
          className="w-full px-4 py-2 border rounded"
          value={form.title}
          onChange={handleChange}
        />
        <textarea
          name="description"
          placeholder="Description"
          className="w-full px-4 py-2 border rounded"
          value={form.description}
          onChange={handleChange}
        />
        <select
          name="categoryId"
          className="w-full px-4 py-2 border rounded"
          value={form.categoryId}
          onChange={handleChange}
        >
          <option value={0}>Select a category</option>
          {categories.map((cat) => (
            <option key={cat.id} value={cat.id}>
              {cat.name}
            </option>
          ))}
        </select>
        <button
          type="submit"
          className="bg-blue-600 text-white py-2 px-4 rounded hover:bg-blue-700"
        >
          {editingId ? "Update Item" : "Add Item"}
        </button>
        {editingId && (
          <button
            type="button"
            onClick={() => {
              setEditingId(null);
              setForm({ title: "", description: "", categoryId: 0 });
            }}
            className="ml-4 text-gray-500 text-sm underline"
          >
            Cancel
          </button>
        )}
        {error && <p className="text-red-500 text-sm">{error}</p>}
      </form>

      {loading ? (
        <p>Loading items...</p>
      ) : items.length === 0 ? (
        <p className="text-gray-600">No items found.</p>
      ) : (
        <ul className="space-y-4">
          {items.map((item) => (
            <li
              key={item.id}
              className="flex justify-between items-center border p-4 rounded"
            >
              <div className="flex-1">
                <h3 className="font-semibold">{item.title}</h3>
                <p className="text-sm text-gray-600 break-words whitespace-pre-wrap max-w-2xl">{item.description}</p>
                <p className="text-sm text-gray-500">
                  Category: {categories.find((c) => c.id === item.categoryId)?.name || "Unknown"}
                </p>
              </div>
              <div className="space-x-2">
                <button
                  onClick={() => handleEdit(item)}
                  className="text-blue-600 hover:underline"
                >
                  Edit
                </button>
                <button
                  onClick={() => handleDelete(item.id)}
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