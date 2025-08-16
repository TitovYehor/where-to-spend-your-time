import { useEffect, useRef, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import type { Category } from "../types/category";
import type { Item } from "../types/item";
import { buildItemQuery, getItems } from "../services/itemService";
import { getCategories } from "../services/categoryService";
import { handleApiError } from "../utils/handleApi";
import type { Tag } from "../types/tag";
import { getTags } from "../services/tagService";
import Select from "react-select";

export default function Home() {
  const [items, setItems] = useState<Item[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [tags, setTags] = useState<Tag[]>([]);
  
  const [searchParams, setSearchParams] = useSearchParams();
  const pageSize = 5;
  const totalCountRef = useRef(0);

  const [filters, setFilters] = useState({
    search: searchParams.get("search") || "",
    categoryId: searchParams.get("categoryId")
      ? parseInt(searchParams.get("categoryId")!)
      : undefined,
    tagsids: searchParams.getAll("tagsids").map((id) => parseInt(id)),
    sortBy: searchParams.get("sortBy") || "",
    descending: searchParams.get("descending") !== "false",
    page: parseInt(searchParams.get("page") || "1", 10),
  });

  const isLastPage = totalCountRef.current <= filters.page * pageSize;
  
  const tagOptions = tags.map(tag => ({ value: tag.id, label: tag.name }));

  useEffect(() => {
    setFilters({
      search: searchParams.get("search") || "",
      categoryId: searchParams.get("categoryId")
        ? parseInt(searchParams.get("categoryId")!)
        : undefined,
      tagsids: searchParams.getAll("tagsids").map((id) => parseInt(id)),
      sortBy: searchParams.get("sortBy") || "",
      descending: searchParams.get("descending") !== "false",
      page: parseInt(searchParams.get("page") || "1", 10),
    });
  }, [searchParams]);

  const updateFilters = (changes: Partial<typeof filters>) => {
    const newFilters = { ...filters, ...changes };
    setFilters(newFilters);
    setSearchParams(buildItemQuery({ ...newFilters, pageSize }), {
      replace: false,
    });
  };

  useEffect(() => {
    const fetchData = async () => {
      try {
        const result = await getItems({ ...filters, pageSize });
        setItems(result.items);
        totalCountRef.current = result.totalCount;
      } catch (e) {
        handleApiError(e);
      }
    };

    fetchData();
  }, [filters]);

  useEffect(() => {
    const fetchMeta = async () => {
      try {
        const cats = await getCategories();
        setCategories(cats);

        const tags = await getTags();
        setTags(tags);
      } catch (e) {
        handleApiError(e);
      }
    };
    
    fetchMeta();
  }, []);

  const searchValue = filters.search;
  const categoryValue = filters.categoryId ?? "";
  const sortValue = filters.sortBy ?? "";

  return (
    <section aria-labelledby="explore-heading" className="max-w-5xl mx-auto px-4 py-6">
      <h1 id="explore-heading" className="text-2xl font-bold mb-4">Explore Items</h1>
      
      <div className="mb-4 text-gray-700">
        {totalCountRef.current} item{totalCountRef.current !== 1 ? "s" : ""} found
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
            value={searchValue}
            onChange={(e) => {
              updateFilters({ search: e.target.value, page: 1 })
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
            value={categoryValue}
            onChange={(e) => {
              updateFilters({ 
                categoryId: e.target.value
                  ? parseInt(e.target.value)
                  : undefined,
                page: 1 })
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
            value={sortValue}
            onChange={(e) => {
              updateFilters({ sortBy: e.target.value, page: 1 })
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
            onClick={() => {
              updateFilters({ descending: !filters.descending, page: 1 })
            }}
            className="px-4 py-2 border rounded-md bg-gray-100 hover:bg-gray-200 transition text-sm w-full sm:w-auto"
          >
            {filters.descending ? "Descending ↓" : "Ascending ↑"}
          </button>
        </div>

        <Select
          isMulti
          options={tagOptions}
          value={tagOptions.filter(opt => filters.tagsids.includes(opt.value))}
          onChange={selected => updateFilters({ 
            tagsids: selected.map(s => s.value), 
            page: 1 })
          }
          placeholder="Select tags..."
          className="w-full sm:w-64"
          classNamePrefix="react-select"
        />
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
              </Link>
            </li>
          ))}
        </ul>
      )}

      <div className="flex justify-center items-center gap-4 mt-6">
        <button
          disabled={filters.page === 1}
          onClick={() => {
            updateFilters({ page: filters.page - 1 })
          }}
          className="px-4 py-2 bg-gray-200 rounded-md disabled:opacity-50"
        >
          Previous
        </button>

        <span className="text-sm text-gray-700">
          Page {filters.page} of {Math.ceil(totalCountRef.current / pageSize)}
        </span>

        <button
          disabled={isLastPage}
          onClick={() => {
            updateFilters({ page: filters.page + 1 })
          }}
          className="px-4 py-2 bg-gray-200 rounded-md disabled:opacity-50"
        >
          Next
        </button>
      </div>

      <p className="mt-4 text-sm text-gray-500 text-center">
        Showing {Math.min((filters.page - 1) * pageSize + 1, totalCountRef.current)}-{Math.min(filters.page * pageSize, totalCountRef.current)} of {totalCountRef.current}
      </p>
    </section>
  );
}