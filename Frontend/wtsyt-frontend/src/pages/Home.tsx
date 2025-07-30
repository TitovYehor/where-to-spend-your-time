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
    <section aria-labelledby="explore-heading" className="max-w-5xl mx-auto px-4 py-6">
      <h1 id="explore-heading" className="text-2xl font-bold mb-4">Explore Items</h1>
      
      <div className="mb-4 text-gray-700">
        {totalCount} item{totalCount !== 1 ? "s" : ""} found
      </div>
      
      <div className="flex flex-wrap gap-4 mb-6 items-end">
        <div className="w-full sm:w-auto">
          <label htmlFor="search" className="block text-sm font-medium text-gray-700 mb-1">
            Search
          </label>
          <input
            id="search"
            type="text"
            placeholder="Search..."
            value={search}
            onChange={(e) => {
              setPage(1);
              setSearch(e.target.value);
            }}
            className="border border-gray-300 px-4 py-2 rounded-md shadow-sm w-full sm:w-auto focus:outline-none focus:ring-2 focus:ring-blue-500"
          />
        </div>

        <div className="w-full sm:w-auto">
          <label htmlFor="categoryId" className="block text-sm font-medium text-gray-700 mb-1">
            Category
          </label>
          <select
            id="categoryId"
            value={categoryId ?? ""}
            onChange={(e) => {
              const val = e.target.value;
              setPage(1);
              setCategoryId(val === "" ? undefined : parseInt(val));
            }}
            className="border border-gray-300 px-4 py-2 rounded-md shadow-sm w-full sm:w-auto focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">All Categories</option>
            {categories.map((cat) => (
              <option key={cat.id} value={cat.id}>
                {cat.name}
              </option>
            ))}
          </select>
        </div>

        <div className="w-full sm:w-auto">
          <label htmlFor="sort" className="block text-sm font-medium text-gray-700 mb-1">
            Sort by
          </label>
          <select
            id="sort"
            value={sortBy ?? ""}
            onChange={(e) => {
              setPage(1);
              setSortBy(e.target.value || undefined);
            }}
            className="border border-gray-300 px-4 py-2 rounded-md shadow-sm w-full sm:w-auto focus:outline-none focus:ring-2 focus:ring-blue-500"
          >
            <option value="">Sort by Default</option>
            <option value="title">Title</option>
            <option value="rating">Rating</option>
          </select>
        </div>

        <div className="w-full sm:w-auto">
          <label className="block text-sm font-medium text-transparent mb-1">Toggle</label>
          <button
            onClick={() => setDescending((prev) => !prev)}
            className="px-4 py-2 border rounded-md bg-gray-100 hover:bg-gray-200 transition text-sm w-full sm:w-auto"
          >
            {descending ? "Descending ↓" : "Ascending ↑"}
          </button>
        </div>
      </div>

      {items.length === 0 ? (
        <p className="text-center text-gray-500">No items found</p>
      ) : (
        <ul className="space-y-4">
          {items.map((item) => (
            <li key={item.id}>
              <Link
                to={`/items/${item.id}`}
                className="block p-4 bg-white rounded-lg shadow hover:shadow-md transition-transform hover:scale-[1.01] focus:outline-none focus:ring-2 focus:ring-blue-500"
              >
                <h3 className="text-lg font-semibold text-gray-800">{item.title}</h3>
                <p className="text-sm text-gray-600">Category: {item.categoryName}</p>
                <p className="text-gray-700 whitespace-pre-line">Description: {item.description}</p>
                <p className="text-yellow-500 font-medium">Rating: {item.averageRating}/5</p>
              </Link>
            </li>
          ))}
        </ul>
      )}

      <div className="flex justify-center items-center gap-4 mt-6">
        <button
          disabled={page === 1}
          onClick={() => setPage((prev) => Math.max(prev - 1, 1))}
          className="px-4 py-2 bg-gray-200 rounded-md disabled:opacity-50"
        >
          Previous
        </button>

        <span className="text-sm text-gray-700">
          Page {page} of {Math.ceil(totalCount / pageSize)}
        </span>

        <button
          disabled={isLastPage}
          onClick={() => setPage((prev) => prev + 1)}
          className="px-4 py-2 bg-gray-200 rounded-md disabled:opacity-50"
        >
          Next
        </button>
      </div>

      <p className="mt-4 text-sm text-gray-500 text-center">
        Showing {Math.min((page - 1) * pageSize + 1, totalCount)}-{Math.min(page * pageSize, totalCount)} of {totalCount}
      </p>
    </section>
  );
}