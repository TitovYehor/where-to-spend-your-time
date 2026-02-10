import { useEffect, useLayoutEffect, useRef, useState } from "react";
import { addCategory, updateCategory, deleteCategory, getPagedCategories } from "../../services/categoryService";
import type { Category } from "../../types/category";
import { handleApiError } from "../../utils/handleApi";
import { Search, PlusCircle, ChevronLeft, ChevronRight, Folders } from "lucide-react";
import CategoryAdminCard from "../../components/categories/CategoryAdminCard";
import Alert from "../../components/common/Alerts";
import { extractProblemDetailsError } from "../../utils/extractProblemDetailsError";

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

  const fetchCategories = async (signal?: AbortSignal) => {
    try {
      setLoading(true);
      const data = await getPagedCategories(
        { search, page, pageSize },
        signal
      );
      setCategories(data.items);
      setTotalCount(data.totalCount);
    } catch (err: any) {
      if (!signal?.aborted) {
        handleApiError(err);
        setError(extractProblemDetailsError(err));
      }
    } finally {
      if (!signal?.aborted) {
        setLoading(false);
      }
    }
  };

  useEffect(() => {
    const controller = new AbortController();

    fetchCategories(controller.signal);

    return () => controller.abort();
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
        setEditingId(null);
        setMessage("Category updated");
      } else {
        await addCategory({ name });
        setMessage("Category added");
      }
      setName("");
      fetchCategories();
      setError("");
    } catch (err: any) {
      handleApiError(err);
      setError(extractProblemDetailsError(err));
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
      setError(extractProblemDetailsError(err));
      setMessage("");
    }
  };

  return (
    <section
      aria-labelledby="manage-categories-heading" 
      className="max-w-4xl mx-auto p-8 bg-white/80 backdrop-blur-md rounded-2xl shadow-xl"
    >
      <h1 className="text-2xl font-bold mb-6 flex items-center gap-2">
        <Folders className="w-6 h-6 text-blue-600" />
        Manage Categories
      </h1>

      <Alert type="success" message={message} onClose={() => setMessage("")} />
      <Alert type="error" message={error} onClose={() => setError("")} />

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
            maxLength={40}
            onChange={(e) => setName(e.target.value)}
            required
          />
          <p className="text-xs text-gray-500 mt-1">
            {name?.length || 0}/40 characters
          </p>
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
        <p className="text-gray-600">No categories found</p>
      ) : (
        <div ref={categoriesRef}>
          <ul className="space-y-6">
            {categories.map((cat) => {
              return (
                <CategoryAdminCard
                  key={cat.id}
                  category={cat}
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