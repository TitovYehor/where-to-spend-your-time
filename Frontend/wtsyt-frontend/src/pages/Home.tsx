import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import type { Category } from "../types/category";
import type { Item } from "../types/item";
import { getItems } from "../services/itemService";
import { getCategories } from "../services/categoryService";
import { handleApiError } from "../utils/handleApi";

export default function Home() {
  const [items, setItems] = useState<Item[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  
  const [searchParams] = useSearchParams();
  const categoryIdParam = searchParams.get("categoryId");
  const [search, setSearch] = useState("");
  const [categoryId, setCategoryId] = useState<number | undefined>(
    categoryIdParam ? parseInt(categoryIdParam) : undefined);
  const [sortBy, setSortBy] = useState<string | undefined>(undefined);
  const [descending, setDescending] = useState(true);
  const [page, setPage] = useState(1);
  const [pageSize] = useState(5);

  const [totalCount, setTotalCount] = useState(0);
  const isLastPage = totalCount <= page * pageSize;

  useEffect(() => {
    const fetchData = async () => {
      try {
        const result = await getItems({
            search: search,
            categoryId: categoryId,
            tagsids: [],
            sortBy: sortBy,
            descending: descending,
            page: page,
            pageSize: pageSize,
          });
        setItems(result.items);
        setTotalCount(result.totalCount);
      } catch (e) {
        handleApiError(e);
      }
    };

    fetchData();
  }, [search, categoryId, sortBy, descending, page]);

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const result = await getCategories();
        setCategories(result);
      } catch (e) {
        handleApiError(e);
      }
    };
    
    fetchCategories();
  }, []);

  return (
    <div className="max-w-5xl mx-auto px-4 py-6">
      <h1 className="text-2xl font-bold mb-4">Explore Items</h1>
      
      <div className="mb-4 text-gray-700">
        {totalCount} item{totalCount !== 1 ? "s" : ""} found
      </div>
      
      <div className="flex flex-wrap gap-4 mb-6">
        <input
          type="text"
          placeholder="Search..."
          value={search}
          onChange={(e) => {
            setPage(1);
            setSearch(e.target.value);
          }}
          className="border px-3 py-2 rounded w-full sm:w-auto"
        />

        <select
          value={categoryId ?? ""}
          onChange={(e) => {
            const val = e.target.value;
            setPage(1);
            setCategoryId(val === "" ? undefined : parseInt(val));
          }}
          className="border px-3 py-2 rounded"
        >
          <option value="">All Categories</option>
          {categories.map((cat) => (
            <option key={cat.id} value={cat.id}>
              {cat.name}
            </option>
          ))}
        </select>

        <select
          value={sortBy ?? ""}
          onChange={(e) => {
            setPage(1);
            setSortBy(e.target.value || undefined);
          }}
          className="border px-3 py-2 rounded"
        >
          <option value="">Sort by Default</option>
          <option value="title">Title</option>
          <option value="rating">Rating</option>
        </select>

        <button
          onClick={() => setDescending((prev) => !prev)}
          className="px-3 py-2 border rounded bg-gray-100"
        >
          {descending ? "Descending ↓" : "Ascending ↑"}
        </button>
      </div>

      {items.length === 0 ? (
        <p>No items found.</p>
      ) : (
        <ul className="space-y-4">
          {items.map((item) => (
            <li key={item.id}>
              <Link
                to={`/items/${item.id}`}
                className="block p-4 bg-white rounded shadow hover:shadow-md transition"
              >
                <h3 className="text-lg font-semibold">{item.title}</h3>
                <p className="text-sm text-gray-600">Category: {item.categoryName}</p>
                <p className="text-gray-800">Description: {item.description}</p>
                <p className="text-yellow-500">Rating: {item.averageRating}/5</p>
              </Link>
            </li>
          ))}
        </ul>
      )}

      <div className="flex justify-center gap-4 mt-6">
        <button
          disabled={page === 1}
          onClick={() => setPage((prev) => Math.max(prev - 1, 1))}
          className="px-4 py-2 bg-gray-200 rounded disabled:opacity-50"
        >
          Previous
        </button>
        <span className="px-4 py-2">Page {page} / {Math.ceil(totalCount / pageSize)}</span>
        <button
          disabled={isLastPage}
          onClick={() => setPage((prev) => prev + 1)}
          className="px-4 py-2 bg-gray-200 rounded disabled:opacity-50"
        >
          Next
        </button>
      </div>

      <div className="text-sm text-gray-500">
        Showing {Math.min((page - 1) * pageSize + 1, totalCount)}–
        {Math.min(page * pageSize, totalCount)} of {totalCount}
      </div>
    </div>
  );
}