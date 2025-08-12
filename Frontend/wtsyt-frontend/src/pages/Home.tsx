import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import type { Category } from "../types/category";
import type { Item } from "../types/item";
import { getItems } from "../services/itemService";
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
  
  const searchParam = searchParams.get("search") || "";
  const categoryIdParam = searchParams.get("categoryId");
  const tagsidsParam = searchParams.getAll("tagsids");
  const sortByParam = searchParams.get("sortBy");
  const descendingParam = searchParams.get("descending");
  const pageParam = parseInt(searchParams.get("page") || "1", 10);

  const [search, setSearch] = useState(searchParam);
  const [categoryId, setCategoryId] = useState<number | undefined>(
    categoryIdParam ? parseInt(categoryIdParam) : undefined
  );
  const [selectedTags, setSelectedTags] = useState<number[]>(
    tagsidsParam.map((id) => parseInt(id))
  );
  const [sortBy, setSortBy] = useState<string | undefined>(sortByParam || undefined);
  const [descending, setDescending] = useState(descendingParam !== "false");
  
  const [page, setPage] = useState(pageParam > 0 ? pageParam : 1);
  const [pageSize] = useState(5);

  const [totalCount, setTotalCount] = useState(0);
  const isLastPage = totalCount <= page * pageSize;

  const tagOptions = tags.map(tag => ({
    value: tag.id,
    label: tag.name
  }));

  const updateParam = (key: string, value?: string) => {
    const newParams = new URLSearchParams(searchParams);
    if (value && value.trim() !== "") {
      newParams.set(key, value);
    } else {
      newParams.delete(key);
    }
    setSearchParams(newParams);
  };

  useEffect(() => {
    const fetchData = async () => {
      try {
        const result = await getItems({
            search: search,
            categoryId: categoryId,
            tagsids: selectedTags,
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
  }, [search, categoryId, selectedTags, sortBy, descending, page]);

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
              const val = e.target.value;
              setPage(1);
              setSearch(val);
              updateParam("search", val);
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
              updateParam("categoryId", val);
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
              const val = e.target.value || undefined;
              setPage(1);
              setSortBy(val);
              updateParam("sortBy", val ?? "");
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
              const newDescending = !descending;
              setDescending(newDescending);
              updateParam("descending", newDescending.toString());
            }}
            className="px-4 py-2 border rounded-md bg-gray-100 hover:bg-gray-200 transition text-sm w-full sm:w-auto"
          >
            {descending ? "Descending ↓" : "Ascending ↑"}
          </button>
        </div>

        <Select
          isMulti
          options={tagOptions}
          value={tagOptions.filter(opt => selectedTags.includes(opt.value))}
          onChange={(selected) => {
            const ids = selected.map(opt => opt.value);
            setPage(1);
            setSelectedTags(ids);

            const newParams = new URLSearchParams(searchParams);
            newParams.delete("tagsids");
            ids.forEach(id => newParams.append("tagsids", id.toString()));
            setSearchParams(newParams);
          }}
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
          disabled={page === 1}
          onClick={() => {
            const newPage = Math.max(page - 1, 1);
            setPage(newPage);
            updateParam("page", newPage.toString());
          }}
          className="px-4 py-2 bg-gray-200 rounded-md disabled:opacity-50"
        >
          Previous
        </button>

        <span className="text-sm text-gray-700">
          Page {page} of {Math.ceil(totalCount / pageSize)}
        </span>

        <button
          disabled={isLastPage}
          onClick={() => {
            const newPage = page + 1;
            setPage(newPage);
            updateParam("page", newPage.toString());
          }}
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