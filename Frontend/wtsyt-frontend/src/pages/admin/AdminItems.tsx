import { useEffect, useRef, useState } from "react";
import { getItems, addItem, updateItem, deleteItem, addTagForItem, removeTagFromItem, getItemById } from "../../services/itemService";
import { getCategories } from "../../services/categoryService";
import type { Item, ItemCreateRequest } from "../../types/item";
import type { Category } from "../../types/category";
import { handleApiError } from "../../utils/handleApi";
import type { Tag } from "../../types/tag";
import { getTags } from "../../services/tagService";

export default function AdminItems() {
  const [items, setItems] = useState<Item[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [tags, setTags] = useState<Tag[]>([]);

  const [loading, setLoading] = useState(true);
  const [form, setForm] = useState<ItemCreateRequest>({
    title: "",
    description: "",
    categoryId: 0,
  });
  
  const [tagInput, setTagInput] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);
  
  const [error, setError] = useState("");

  const formRef = useRef<HTMLFormElement>(null);

  const fetchItemsAndCategories = async () => {
    try {
      const [itemList, categoryList, tagList] = await Promise.all([
        getItems({}),
        getCategories(),
        getTags(),
      ]);
      setItems(itemList.items);
      setCategories(categoryList);
      setTags(tagList);
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
        const updatedItem = await getItemById(editingId);
        setItems((prev) =>
          prev.map((i) => (i.id === editingId ? updatedItem : i))
        );
      } else {
        const newItem = await addItem(form);
        setItems((prev) => [...prev, newItem]);
        setEditingId(newItem.id);
      }

      setError("");
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
      setEditingId(null);
      setForm({ title: "", description: "", categoryId: 0 });
      setError("");
      fetchItemsAndCategories();
    } catch {
      setError("Failed to delete item");
    }
  };

  const handleAddTag = async () => {
    if (!editingId || !tagInput.trim()) return;

    try {
      const addedTag = await addTagForItem(editingId, { name: tagInput.trim() });
      
      setItems((prev) =>
        prev.map((item) =>
          item.id === editingId
            ? { ...item, tags: [...item.tags, addedTag] }
            : item
        )
      );

      setTagInput("");
      setError("");
    } catch {
      setError("Failed to add tag");
    }
  };

  const handleRemoveTag = async (tagName: string) => {
    if (!editingId || !tagName) return;

    try {
      await removeTagFromItem(editingId, tagName);
    
      setItems((prev) =>
        prev.map((item) =>
          item.id === editingId
            ? { ...item, tags: item.tags.filter((t) => t.name !== tagName) }
            : item
        )
      );

      setError("");
    } catch {
      setError("Failed to remove tag");
    }
  };

  return (
    <div className="max-w-4xl mx-auto p-6 bg-white rounded-xl shadow">
      <h1 className="text-2xl font-bold mb-6">Manage Items</h1>

      <form ref={formRef} onSubmit={handleSubmit} className="space-y-4 mb-8">
        {error && (
          <p className="text-red-500 text-sm">
            {error}
            </p>
        )}

        <div>
          <label htmlFor="title" className="block text-sm font-medium text-gray-700 mb-1">
            Title
          </label>
          <input
            id="title"
            name="title"
            type="text"
            placeholder="Title"
            className="w-full px-4 py-2 border rounded"
            value={form.title}
            onChange={handleChange}
          />
        </div>
        
        <div>
          <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-1">
            Description
          </label>
          <textarea
            id="description"
            name="description"
            placeholder="Description"
            className="w-full px-4 py-2 border rounded"
            value={form.description}
            onChange={handleChange}
          />
        </div>
        
        <div>
          <label htmlFor="categoryId" className="block text-sm font-medium text-gray-700 mb-1">
            Category
          </label>
          <select
            id="categoryId"
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
        </div>

        <button
          type="submit"
          className="bg-blue-600 text-white py-2 px-4 rounded hover:bg-blue-700"
        >
          {editingId ? "Update Item" : "Add Item"}
        </button>
        
        {editingId && (
          <div className="mt-6 space-y-4">
            <button
              type="button"
              onClick={() => handleDelete(editingId)}
              className="bg-red-600 text-white py-2 px-4 rounded hover:bg-red-700 transition-colors"
            >
              Delete item
            </button>
            <div className="flex flex-col sm:flex-row sm:items-center sm:gap-4">
              
              <div>
                <label htmlFor="tagsList" className="block text-sm font-medium text-gray-700 mb-1">
                  Tag
                </label>
                <select
                  id="tagsList"
                  name="tagsSelector"
                  className="w-full px-4 py-2 border rounded"
                  value={tagInput}
                  onChange={(e) => setTagInput(e.target.value)}
                >
                  <option value={0}>Select a tag</option>
                  {tags.map((t) => (
                    <option key={t.id} value={t.name}>
                      {t.name}
                    </option>
                  ))}
                </select>
              </div>

              <button
                type="button"
                className="mt-6 px-5 py-2 bg-green-600 text-white rounded hover:bg-green-700 transition"
                onClick={handleAddTag}
              >
                Add Tag
              </button>
            </div>

            <div className="flex flex-wrap gap-2">
              {items.find((i) => i.id === editingId)?.tags.map((tag) => (
                <span
                  key={tag.id}
                  className="flex items-center bg-gray-200 text-sm px-3 py-1 rounded-full"
                >
                  {tag.name}
                  <button
                    onClick={() => handleRemoveTag(tag.name)}
                    className="ml-2 text-red-500 hover:text-red-700 font-semibold"
                    title="Remove tag"
                  >
                    &times;
                  </button>
                </span>
              ))}
            </div>

            <div>
              <button
                type="button"
                onClick={() => {
                  setEditingId(null);
                  setForm({ title: "", description: "", categoryId: 0 });
                }}
                className="text-gray-500 text-sm underline hover:text-gray-700"
              >
                Cancel editing
              </button>
            </div>
          </div>
        )}
      </form>

      {loading ? (
        <p>Loading items...</p>
      ) : items.length === 0 ? (
        <p className="text-gray-600">No items found</p>
      ) : (
        <ul className="space-y-4">
          {items.map((item) => (
            <li
              key={item.id}
              className="flex flex-col sm:flex-row justify-between items-center bg-gray-50 border rounded-xl p-4 shadow-sm"
            >
              <div className="flex-1 text-left">
                <h3 className="font-semibold text-lg">{item.title}</h3>
                <p className="text-sm text-gray-600 mt-1 mb-2 whitespace-pre-wrap max-w-3xl line-clamp-3">{item.description}</p>
                <p className="text-sm text-gray-500">
                  <span className="font-medium">Category:</span>{" "}
                  {categories.find((c) => c.id === item.categoryId)?.name || "Unknown"}
                </p>

                {item.tags && item.tags.length > 0 && (
                  <div className="mt-3 flex flex-wrap gap-2">
                    {item.tags.map((tag) => (
                      <span
                        key={tag.id}
                        className="bg-blue-100 text-blue-800 text-xs font-medium px-3 py-1 rounded-full"
                      >
                        {tag.name}
                      </span>
                    ))}
                  </div>
                )}
              </div>

              <div className="mt-4 sm:mt-0 sm:ml-6 flex flex-col gap-3 items-center">
                <button
                  onClick={() => handleEdit(item)}
                  className="text-blue-600 hover:underline font-medium"
                  aria-label={`Edit ${item.title}`}
                >
                  Edit
                </button>
                <button
                  onClick={() => handleDelete(item.id)}
                  className="text-red-600 hover:underline font-medium"
                  aria-label={`Delete ${item.title}`}
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