import { useEffect, useLayoutEffect, useRef, useState } from "react";
import { addCategory, updateCategory, deleteCategory, getPagedCategories } from "../../services/categoryService";
import type { Category } from "../../types/category";
import { handleApiError } from "../../utils/handleApi";
import type { CategoryPagedResult } from "../../types/pagination/pagedResult";
import { Layers, Folder, Pencil, Trash2, Search, PlusCircle, ChevronLeft, ChevronRight } from "lucide-react";

export default function AdminCategories() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [totalCount, setTotalCount] = useState(0);

  const [loading, setLoading] = useState(true);

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize] = useState(5);

  const [name, setName] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);
  
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");

  const formRef = useRef<HTMLFormElement>(null);

  const categoriesRef = useRef<HTMLDivElement | null>(null);

  const [categoryPageChanged, setCategoryPageChanged] = useState(false);

  const fetchCategories = async () => {
    try {
      setLoading(true);
      const data: CategoryPagedResult = await getPagedCategories({
        search,
        page,
        pageSize
      });
      setCategories(data.items);
      setTotalCount(data.totalCount);
    } catch (err) {
      handleApiError(err);
      setError("Failed to load categories");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCategories();
  }, [search, page, pageSize]);

  useLayoutEffect(() => {
    if (categoryPageChanged && categories.length > 0 && categoriesRef.current) {
      const y = categoriesRef.current.getBoundingClientRect().top + window.scrollY - 60;
      window.scrollTo({ top: y, behavior: "smooth" });
      setCategoryPageChanged(false);
    }
  }, [categories]);

  const totalPages = Math.ceil(totalCount / pageSize);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (name.trim().length < 2) {
      setError("Category name must be at least 2 characters");
      return;
    }

    try {
      if (editingId !== null) {
        await updateCategory(editingId, { name });
        fetchCategories()
        setEditingId(null);
        setMessage("Category updated");
      } else {
        await addCategory({ name });
        fetchCategories()
        setMessage("Category added");
      }
      setName("");
      fetchCategories();
      setError("");
    } catch (err) {
      handleApiError(err);
      setError("Failed to save category");
      setMessage("");
    }
  };

  const handleEdit = (category: Category) => {
    setEditingId(category.id);
    setName(category.name);
    
    setTimeout(() => {
        formRef.current?.scrollIntoView({ behavior: "smooth", block: "center" });
    }, 0);
    
    setError("");
    setMessage("");
  };

  const handleDelete = async (id: number) => {
    if (!confirm("Are you sure you want to delete this category?")) return;

    try {
      await deleteCategory(id);
      fetchCategories();
      setMessage("Category deleted");
      setError("");
      setEditingId(null);
      setName("");
    } catch (err) {
      handleApiError(err);
      setError("Failed to delete category");
      setMessage("");
    }
  };

  const filteredCategories = categories.filter(cat =>
    cat.name.toLowerCase().includes(search.toLowerCase())
  );

  return (
    <section
      aria-labelledby="manage-categories-heading" 
      className="max-w-4xl mx-auto p-8 bg-white/80 backdrop-blur-md rounded-2xl shadow-xl"
    >
      <h1 className="text-2xl font-bold mb-6 flex items-center gap-2">
        <Layers className="w-6 h-6 text-blue-600" />
        Manage Categories
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
          <label htmlFor="categoryName" className="block text-sm font-medium text-black mb-1">
            Category Name
          </label>
          <input
            id="categoryName"
            type="text"
            placeholder="e.g., Books"
            className="w-full px-4 py-2 border rounded"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>
        <div className="flex items-center gap-4">
          <button
            type="submit"
            className="flex items-center gap-2 bg-blue-600 hover:bg-blue-700 text-white py-2 px-4 rounded"
          >
            <PlusCircle className="w-4 h-4" />
            {editingId ? "Update Category" : "Add Category"}
          </button>
          {editingId && (
            <button
              type="button"
              className="text-sm text-gray-500 underline"
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
        <label htmlFor="search" className="block text-sm font-medium text-black mb-1">
          Search
        </label>
        <div className="relative">
          <Search className="absolute left-3 top-2.5 w-4 h-4 text-gray-400" />
          <input
            id="search"
            type="text"
            placeholder="Search categories..."
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
              setPage(1);
            }}
            className="w-full border border-gray-300 pl-10 pr-4 py-2 rounded-lg shadow-sm mb-6 focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>
      </div>

      {loading ? (
        <p>Loading...</p>
      ) : categories.length === 0 ? (
        <p className="text-gray-600">No categories found.</p>
      ) : (
        <div ref={categoriesRef}>
          <ul className="space-y-6">
            {filteredCategories.map((cat) => (
              <li
                key={cat.id}
                className="flex flex-col sm:flex-row justify-between items-center bg-gray-50 border rounded-xl p-4 shadow-sm"
              >
                <div className="flex-1 flex items-center gap-2">
                  <Folder className="w-5 h-5 text-gray-500" />
                  <span className="text-lg font-medium text-gray-900">{cat.name}</span>
                </div>
                
                <div className="mt-3 sm:mt-0 sm:ml-6 flex gap-4">
                  <button
                    onClick={() => handleEdit(cat)}
                    className="flex items-center gap-1 text-blue-600 hover:underline font-medium"
                  >
                    <Pencil className="w-4 h-4" />
                    Edit
                  </button>
                  <button
                    onClick={() => handleDelete(cat.id)}
                    className="flex items-center gap-1 text-red-600 hover:underline font-medium"
                  >
                    <Trash2 className="w-4 h-4" />
                    Delete
                  </button>
                </div>
              </li>
            ))}
          </ul>

          <div className="flex justify-center items-center gap-3 mt-6">
            <button
              disabled={page === 1}
              onClick={(e) => {
                e.preventDefault();
                setPage((p) => Math.max(p - 1, 1));
                setCategoryPageChanged(true);
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
                setCategoryPageChanged(true);
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