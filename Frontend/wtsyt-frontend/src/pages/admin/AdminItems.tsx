import { useEffect, useRef, useState } from "react";
import { getItems, addItem, updateItem, deleteItem, addTagForItem, removeTagFromItem, getItemById } from "../../services/itemService";
import { getCategories } from "../../services/categoryService";
import type { Item, ItemCreateRequest } from "../../types/item";
import type { Category } from "../../types/category";
import { handleApiError } from "../../utils/handleApi";
import type { Tag } from "../../types/tag";
import { getTags } from "../../services/tagService";
import Select from "react-select";

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
  
  const [search, setSearch] = useState("");
  const [tagInput, setTagInput] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);
  
  const categoryOptions = categories.map((cat) => ({ value: cat.id, label: cat.name }));
  const tagOptions = [
    { value: "", label: "Choose tag" },
    ...tags.map(tag => ({ value: tag.name, label: tag.name }))
  ];

  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

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

  const handleSelectChange = (field: keyof typeof form) => 
  (selectedOption: { value: any; label: string } | null) => {
    if (selectedOption) {
      setForm({ ...form, [field]: selectedOption.value });
    }
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
        setMessage("Item updated11");
      } else {
        const newItem = await addItem(form);
        setItems((prev) => [...prev, newItem]);
        setEditingId(newItem.id);
        setMessage("Item added");
      }

      setError("");
    } catch (err) {
      handleApiError(err);
      setError("Failed to save item");
      setMessage("");
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
    setMessage("");
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Are you sure you want to delete this item?")) return;

    try {
      await deleteItem(id);
      setEditingId(null);
      setForm({ title: "", description: "", categoryId: 0 });
      setError("");
      setMessage("Item deleted");
      fetchItemsAndCategories();
    } catch {
      setError("Failed to delete item");
      setMessage("");
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
      setMessage("Tag added");
    } catch {
      setError("Failed to add tag");
      setMessage("");
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
      setMessage("Tag removed");
    } catch {
      setError("Failed to remove tag");
      setMessage("");
    }
  };

  const filteredItems = items.filter(i =>
    i.title.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <div className="max-w-4xl mx-auto p-6 bg-white rounded-xl shadow">
      <h1 className="text-2xl font-bold mb-6">Manage Items</h1>

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

      <form ref={formRef} onSubmit={handleSubmit} className="space-y-4 mb-8">
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
          <Select
            id="categoryId"
            options={categoryOptions}
            value={
              categoryOptions.find(
                (opt) => opt.value === (form.categoryId ?? 0)
              ) || categoryOptions[0]
            }
            onChange={handleSelectChange("categoryId")}
            classNamePrefix="react-select"
          />
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
                <Select
                  inputId="tagsList"
                  options={tagOptions}
                  value={
                    tagOptions.find(
                      (opt) => opt.value === (tagInput ?? "")
                    ) || tagOptions[0]}
                  onChange={option => setTagInput(option?.value ?? "")}
                  placeholder="Select tags..."
                  className="w-full sm:w-64"
                  classNamePrefix="react-select"
                />
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
                    type="button"
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

      <div className="mb-3">
        <label htmlFor="search" className="block text-sm font-medium text-gray-700 mb-1">
          Search
        </label>
        <input
          id="search"
          type="text"
          placeholder="Search items..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="w-full border border-gray-300 px-4 py-2 rounded-lg shadow-sm mb-6 focus:outline-none focus:ring-2 focus:ring-blue-500"
        />
      </div>

      {loading ? (
        <p>Loading items...</p>
      ) : items.length === 0 ? (
        <p className="text-gray-600">No items found</p>
      ) : (
        <ul className="space-y-4">
          {filteredItems.map((item) => (
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