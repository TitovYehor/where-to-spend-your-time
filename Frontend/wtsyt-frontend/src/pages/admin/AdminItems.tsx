import { useEffect, useLayoutEffect, useRef, useState } from "react";
import { useAutoResizeTextareas } from "../../hooks/useAutoResizeTextareas";
import { getItems, addItem, updateItem, deleteItem, addTagForItem, removeTagFromItem, getItemById } from "../../services/itemService";
import { getCategories } from "../../services/categoryService";
import type { Item, ItemCreateRequest } from "../../types/item";
import type { Category } from "../../types/category";
import { handleApiError } from "../../utils/handleApi";
import type { Tag } from "../../types/tag";
import { getTags } from "../../services/tagService";
import Select from "react-select";
import type { MediaType } from "../../types/media";
import { deleteMedia, getMediaUrl, uploadMedia } from "../../services/mediaService";
import { Boxes, ChevronLeft, ChevronRight, PlusCircle, Search } from "lucide-react";
import ItemAdminCard from "../../components/items/ItemAdminCard";
import Alert from "../../components/common/Alerts";

export default function AdminItems() {
  const [items, setItems] = useState<Item[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [categories, setCategories] = useState<Category[]>([]);
  const [tags, setTags] = useState<Tag[]>([]);

  const [loading, setLoading] = useState(true);
  const [form, setForm] = useState<ItemCreateRequest>({
    title: "",
    description: "",
    categoryId: 0,
  });
  
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize] = useState(5);

  const [tagInput, setTagInput] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);

  const [mediaPreview, setMediaPreview] = useState<string | null>(null);

  const [mediaFile, setMediaFile] = useState<File | null>(null);
  const [mediaType, setMediaType] = useState<MediaType>("Image");
  
  const categoryOptions =  [
    { value: "", label: "Choose category" },
    ...categories.map(cat => ({ value: cat.id, label: cat.name }))
  ];
  
  const tagOptions = [
    { value: "", label: "Choose tag" },
    ...tags.map(tag => ({ value: tag.name, label: tag.name }))
  ];

  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  const formRef = useRef<HTMLFormElement>(null);
  const itemsRef = useRef<HTMLDivElement | null>(null);

  const titleRef = useRef<HTMLTextAreaElement | null>(null);
  const descriptionRef = useRef<HTMLTextAreaElement | null>(null);

  const [itemPageChanged, setItemPageChanged] = useState(false);

  const fetchItemsAndMeta = async (signal?: AbortSignal) => {
    try {
      const [itemList, categoryList, tagList] = await Promise.all([
        getItems({
          search: search,
          page: page,
          pageSize: pageSize
        }, signal),
        getCategories(signal),
        getTags(signal),
      ]);
      setItems(itemList.items);
      setTotalCount(itemList.totalCount);
      setCategories(categoryList);
      setTags(tagList);
    } catch (err) {
      if (!signal?.aborted) {
        handleApiError(err);
        setError("Failed to fetch data");
      }
    } finally {
      if (!signal?.aborted) {
        setLoading(false);
      }
    }
  };
  
  useAutoResizeTextareas([titleRef, descriptionRef], [form.title, form.description]);

  useEffect(() => {
    const controller = new AbortController();

    fetchItemsAndMeta(controller.signal);

    return () => controller.abort();
  }, [search, page, pageSize]);

  useLayoutEffect(() => {
    if (itemPageChanged && items.length > 0 && itemsRef.current) {
      const y = itemsRef.current.getBoundingClientRect().top + window.scrollY - 160;
      window.scrollTo({ top: y, behavior: "smooth" });
      setItemPageChanged(false);
    }
  }, [items]);

  const totalPages = Math.ceil(totalCount / pageSize);

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
        setMessage("Item updated");
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
      fetchItemsAndMeta();
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

  const handleUploadMedia = async () => {
    if (!editingId || !mediaFile) return;

    try {
      await uploadMedia({
        itemId: editingId,
        type: mediaType,
        file: mediaFile,
      });

      const updatedItem = await getItemById(editingId);
      setItems((prev) =>
        prev.map((i) => (i.id === editingId ? updatedItem : i))
      );

      setMessage("Media uploaded successfully");
      setError("");
      setMediaFile(null);
      setMediaPreview(null);
    } catch (err) {
      handleApiError(err);
      setError("Failed to upload media");
      setMessage("");
    }
  };

  const handleDeleteMedia = async (mediaId: number) => {
    if (!editingId) return;
    if (!confirm("Are you sure you want to delete this media?")) return;

    try {
      const success = await deleteMedia(mediaId);
      if (success) {
        const updatedItem = await getItemById(editingId);
        setItems((prev) =>
          prev.map((i) => (i.id === editingId ? updatedItem : i))
        );
        setMessage("Media deleted successfully");
        setError("");
      } else {
        setError("Failed to delete media");
        setMessage("");
      }
    } catch (err) {
      handleApiError(err);
      setError("Error deleting media");
      setMessage("");
    }
  };

  return (
    <section 
      aria-labelledby="manage-items-heading" 
      className="max-w-4xl mx-auto p-8 bg-white/80 backdrop-blur-md rounded-2xl shadow-xl"
    >
      <h1 className="text-2xl font-bold mb-6 flex items-center gap-2">
        <Boxes className="w-6 h-6 text-indigo-500" />
        Manage Items
      </h1>

      <Alert type="success" message={message} onClose={() => setMessage("")} />
      <Alert type="error" message={error} onClose={() => setError("")} />
      
      <form ref={formRef} onSubmit={handleSubmit} className="space-y-4 mb-8">
        <div>
          <label htmlFor="title" className="block text-sm font-medium text-black mb-1">
            Title
          </label>
          <textarea
            ref={titleRef}
            id="title"
            name="title"
            placeholder="Title"
            className="w-full px-4 py-2 border rounded"
            value={form.title}
            maxLength={100}
            onChange={(e) => {
              handleChange(e);
            }}
            required
          />
          <p className="text-xs text-gray-500 mt-1">
            {form.title?.length || 0}/100 characters
          </p>
        </div>
        
        <div>
          <label htmlFor="description" className="block text-sm font-medium text-black mb-1">
            Description
          </label>
          <textarea
            ref={descriptionRef}
            id="description"
            name="description"
            placeholder="Description"
            className="w-full px-4 py-2 border rounded"
            value={form.description}
            maxLength={500}
            onChange={(e) => {
              handleChange(e);
            }}
            required
          />
          <p className="text-xs text-gray-500 mt-1">
            {form.description?.length || 0}/500 characters
          </p>
        </div>
        
        <div>
          <label htmlFor="categoryId" className="block text-sm font-medium text-black mb-1">
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
          className="flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white py-2 px-4 rounded"
        >
          <PlusCircle className="w-4 h-4" />
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
                  className="flex items-center bg-blue-200 text-green-800 text-sm px-3 py-1 rounded-full"
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
              <h3 className="text-sm font-medium text-gray-700 mb-2">Uploaded Media</h3>
              {items.find((i) => i.id === editingId)?.media.length === 0 ? (
                <p className="text-sm text-gray-500 italic">No media uploaded yet</p>
              ) : (
                <div className="flex flex-wrap gap-4">
                  {items.find((i) => i.id === editingId)?.media.map((m) => (
                    <div key={m.id} className="relative group flex flex-col items-center">
                      <button
                        type="button"
                        onClick={() => handleDeleteMedia(m.id)}
                        className="absolute top-1 right-1 bg-red-600 text-white rounded p-1 text-xs opacity-80 hover:opacity-100 transition hidden group-hover:block"
                        title="Delete media"
                      >
                        ✕
                      </button>
                      
                      {m.type === "Image" ? (
                        <img
                          src={getMediaUrl(m.url)}
                          alt="Media"
                          className="w-32 h-32 object-cover rounded-lg shadow cursor-pointer"
                          onClick={() => window.open(getMediaUrl(m.url), "_blank")}
                        />
                      ) : (
                        <video
                          src={getMediaUrl(m.url)}
                          controls
                          className="w-64 h-40 object-cover rounded-lg shadow cursor-pointer"
                          onClick={() => window.open(getMediaUrl(m.url), "_blank")}
                        />
                      )}
                    </div>
                  ))}
                </div>
              )}
            </div>

            <div className="mt-6 space-y-4">
              <label className="block text-sm font-medium text-gray-700">Upload Media</label>
              
              <select
                value={mediaType}
                onChange={(e) => setMediaType(e.target.value as "Image" | "Video")}
                className="border rounded px-3 py-2 bg-blue-50"
              >
                <option value="Image">Image</option>
                <option value="Video">Video</option>
              </select>

              <input
                type="file"
                accept={mediaType === "Image" ? "image/*" : "video/*"}
                onChange={(e) => {
                  const file = e.target.files?.[0] || null;
                  setMediaFile(file);
                  if (file) {
                    const url = URL.createObjectURL(file);
                    setMediaPreview(url);
                  } else {
                    setMediaPreview(null);
                  }
                }}
                className="block w-full text-sm text-gray-500 file:mr-4 file:py-2 
                          file:px-4 file:rounded-full file:border-0 
                          file:text-sm file:font-semibold file:bg-blue-50 
                          file:text-blue-700 hover:file:bg-blue-100"
              />

              {mediaPreview && (
                <div className="mt-4">
                  {mediaType === "Image" ? (
                    <img
                      src={mediaPreview}
                      alt="Preview"
                      className="w-32 h-32 object-cover rounded-lg shadow cursor-pointer"
                      onClick={() => window.open(mediaPreview, "_blank")}
                    />
                  ) : (
                    <video
                      src={mediaPreview}
                      controls
                      className="w-48 rounded-lg shadow cursor-pointer"
                      onClick={() => window.open(mediaPreview, "_blank")}
                    />
                  )}
                </div>
              )}

              <button
                type="button"
                onClick={handleUploadMedia}
                disabled={!mediaFile}
                className="bg-indigo-600 text-white px-4 py-2 rounded hover:bg-indigo-700 disabled:opacity-50"
              >
                Upload
              </button>
            </div>

            <div>
              <button
                type="button"
                onClick={() => {
                  setEditingId(null);
                  setForm({ title: "", description: "", categoryId: 0 });
                  setMediaPreview(null);
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
        <label htmlFor="search" className="block text-sm font-medium text-black mb-1">
          Search
        </label>
        <div className="relative">
          <Search className="absolute left-3 top-2.5 w-4 h-4 text-gray-400" />
          <input
            id="search"
            type="text"
            placeholder="Search items..."
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
              setPage(1);
            }}
            className="w-full border border-gray-300 pl-10 px-4 py-2 rounded-lg shadow-sm mb-6 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>
      </div>

      {loading ? (
        <p>Loading items...</p>
      ) : items.length === 0 ? (
        <p className="text-gray-600">No items found</p>
      ) : (
        <div ref={itemsRef}>
          <ul className="space-y-4">
            {items.map((item) => {
              return (
                <ItemAdminCard
                  key={item.id}
                  item={item}
                  onEdit={handleEdit}
                  onDelete={handleDelete}
                />
              );
            })}
          </ul>

          <div className="flex justify-center items-center gap-3 mt-6">
            <button
              disabled={page === 1}
              onClick={(e) => {
                e.preventDefault();
                setPage((p) => Math.max(p - 1, 1));
                setItemPageChanged(true);
              }}
              className="p-2 rounded bg-gray-200 disabled:opacity-50"
              aria-label="Previous page"
            >
              <ChevronLeft className="w-5 h-5" />
            </button>

            <span>
              Page {page} of {totalPages}
            </span>

            <button
              disabled={page === totalPages}
              onClick={(e) => {
                e.preventDefault();
                setPage((p) => Math.min(p + 1, totalPages));
                setItemPageChanged(true);
              }}
              className="p-2 rounded bg-gray-200 disabled:opacity-50"
              aria-label="Next page"
            >
              <ChevronRight className="w-5 h-5" />
            </button>
          </div>
        </div>
      )}
    </section>
  );
}