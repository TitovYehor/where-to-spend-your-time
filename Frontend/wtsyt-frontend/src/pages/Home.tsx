import { useEffect, useLayoutEffect, useRef, useState } from "react";
import { useSearchParams } from "react-router-dom";
import type { Category } from "../types/category";
import type { Item } from "../types/item";
import { buildItemQuery, getItems } from "../services/itemService";
import { getCategories } from "../services/categoryService";
import { handleApiError } from "../utils/handleApi";
import type { Tag } from "../types/tag";
import { getTags } from "../services/tagService";
import Select from "react-select";
import { Search, SortAsc, SortDesc, Tags, Folder, ChevronLeft, ChevronRight, Boxes } from "lucide-react";
import ItemFullCard from "../components/items/ItemFullCard";

export default function Home() {
  const [items, setItems] = useState<Item[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [tags, setTags] = useState<Tag[]>([]);
  
  const [pageChangedByUser, setPageChangedByUser] = useState(false);

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
  
  const categoryOptions = [
    { value: 0, label: "All Categories" },
    ...categories.map((cat) => ({ value: cat.id, label: cat.name })),
  ];
  const sortOptions = [
    { value: "", label: "Default" },
    { value: "title", label: "Title" },
    { value: "rating", label: "Rating" },
    { value: "popularity", label: "Popularity" },
  ];

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

  useLayoutEffect(() => {
    if (pageChangedByUser && items.length > 0) { 
      window.scrollTo({ top: 0, behavior: "auto" });
      setPageChangedByUser(false);
    }
  }, [items, pageChangedByUser]);

  const updateFilters = (changes: Partial<typeof filters>, userTriggered = false) => {
    const newFilters = { ...filters, ...changes };
    setFilters(newFilters);
    setSearchParams(buildItemQuery({ ...newFilters, pageSize }), {
      replace: false,
    });
    if (userTriggered) {
      setPageChangedByUser(true);
    }
  };

  const searchValue = filters.search;

  return (
    <section 
      aria-labelledby="explore-heading" 
      className="max-w-4xl mx-auto p-8 bg-white/80 backdrop-blur-md rounded-2xl shadow-xl"
    >
      <h1 id="explore-heading" className="text-3xl font-bold mb-6 text-black flex items-center gap-2">
        <Boxes className="w-7 h-7 text-indigo-500" />
        Explore Items
      </h1>

      <div className="mb-4 text-black">
        {totalCountRef.current} item{totalCountRef.current !== 1 ? "s" : ""} found
      </div>
      
      <div className="flex flex-wrap gap-4 mb-6 items-end">
        <div className="flex-1 min-w-[200px]">
          <label htmlFor="search" className="block text-sm font-medium text-black mb-1 flex items-center gap-1">
            <Search className="w-4 h-4 text-gray-500" />
            Search
          </label>
          <input
            id="search"
            type="text"
            placeholder="Search items..."
            value={searchValue}
            onChange={(e) => {
              updateFilters({ search: e.target.value, page: 1 });
            }}
            className="w-full rounded-md border border-gray-300 bg-white px-3 py-1.5 pr-10 shadow-sm 
                      focus:border-blue-500 focus:ring-2 focus:ring-blue-500 focus:ring-offset-1 
                      transition"
          />
        </div>

        <div className="min-w-[180px] flex-1 sm:flex-none">
          <label htmlFor="categoryId" className="block text-sm font-medium text-black mb-1 flex items-center gap-1">
            <Folder className="w-4 h-4 text-blue-500" />
            Category
          </label>
          <Select
            id="categoryId"
            options={categoryOptions}
            value={
              categoryOptions.find(
                (opt) => opt.value === (filters.categoryId ?? 0)
              ) || categoryOptions[0]
            }
            onChange={(option) =>
              updateFilters({
                categoryId:
                  option && option.value !== 0 ? Number(option.value) : undefined,
                page: 1,
              })
            }
            classNamePrefix="react-select"
          />
        </div>

        <div className="min-w-[160px] flex-1 sm:flex-none">
          <label htmlFor="sort" className="block text-sm font-medium text-black mb-1 flex items-center gap-1">
            <SortAsc className="w-4 h-4 text-gray-500" />
            Sort by
          </label>
          <Select 
            id="sort"
            options={sortOptions}
            value={
              sortOptions.find((opt) => opt.value === filters.sortBy)
            }
            onChange={(option) => 
              updateFilters({
                sortBy: option?.value || "",
                page: 1
              }) 
            }
            classNamePrefix="react-select"
          />
        </div>

        <div className="min-w-[140px] flex-1 sm:flex-none">
          <label className="block text-sm font-medium text-black mb-1">
            Order
          </label>
          <div className="inline-flex w-full rounded-md shadow-sm border border-gray-300">
            <button
              onClick={() => updateFilters({ descending: false, page: 1 })}
              className={`flex-1 px-3 py-2 text-sm flex items-center justify-center gap-1 rounded-l-md transition ${
                !filters.descending
                  ? "bg-blue-500 text-white"
                  : "bg-white text-gray-700 hover:bg-gray-50"
              }`}
            >
              <SortAsc className="w-4 h-4" />
              Asc
            </button>
            <button
              onClick={() => updateFilters({ descending: true, page: 1 })}
              className={`flex-1 px-3 py-2 text-sm flex items-center justify-center gap-1 rounded-r-md transition ${
                filters.descending
                  ? "bg-blue-500 text-white"
                  : "bg-white text-gray-700 hover:bg-gray-50"
              }`}
            >
              <SortDesc className="w-4 h-4" />
              Desc
            </button>
          </div>
        </div>

        <div className="min-w-[200px] max-w-[600px] flex-1 sm:flex-none">
          <label htmlFor="tags" className="block text-sm font-medium text-black mb-1 flex items-center gap-1">
            <Tags className="w-4 h-4 text-green-500" />
            Tags
          </label>
          <Select
            inputId="tags"
            isMulti
            options={tagOptions}
            value={tagOptions.filter(opt => filters.tagsids.includes(opt.value))}
            onChange={selected => updateFilters({ 
              tagsids: selected.map(s => s.value), 
              page: 1 })
            }
            placeholder="Select tags..."
            className="w-full sm:w-auto"
            classNamePrefix="react-select"
          />
        </div>
      </div>

      {items.length === 0 ? (
        <p className="text-center text-black">No items found</p>
      ) : (
        <ul className="space-y-4">
          {items.map((item) => (
            <ItemFullCard key={item.id} item={item} />
          ))}
        </ul>
      )}

      <div className="flex justify-center items-center gap-4 mt-6">
        <button
          disabled={filters.page === 1}
          onClick={() => {
            updateFilters({ page: filters.page - 1 }, true);
          }}
          className="p-2 rounded bg-gray-200 disabled:opacity-50"
          aria-label="Previous page"
        >
          <ChevronLeft className="w-5 h-5" />
        </button>

        <span className="text-sm text-black">
          Page {filters.page} of {Math.ceil(totalCountRef.current / pageSize)}
        </span>

        <button
          disabled={isLastPage}
          onClick={() => {
            updateFilters({ page: filters.page + 1 }, true);
          }}
          className="p-2 rounded bg-gray-200 disabled:opacity-50"
          aria-label="Next page"
        >
          <ChevronRight className="w-5 h-5" />
        </button>
      </div>

      <p className="mt-4 text-sm text-gray-700 text-center">
        Showing {Math.min((filters.page - 1) * pageSize + 1, totalCountRef.current)}-{Math.min(filters.page * pageSize, totalCountRef.current)} of {totalCountRef.current}
      </p>
    </section>
  );
}