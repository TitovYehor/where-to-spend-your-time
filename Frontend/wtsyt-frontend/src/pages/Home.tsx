import { useEffect, useState } from "react";
import { Link } from "react-router-dom";

type Item = {
  id: number;
  title: string;
  description: string;
  averageRating: number;
  categoryName: string;
};

type Category = {
  id: number;
  name: string;
};

export default function Home() {
  const [items, setItems] = useState<Item[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [search, setSearch] = useState("");
  const [categoryId, setCategoryId] = useState<number | undefined>(undefined);
  const [sortBy, setSortBy] = useState<string | undefined>(undefined);
  const [descending, setDescending] = useState(true);

  const fetchItems = async () => {
    const query = new URLSearchParams();
    if (search) query.append("search", search);
    if (categoryId !== undefined) query.append("categoryId", categoryId.toString());
    if (sortBy) query.append("sortBy", sortBy);
    query.append("descending", descending.toString());

    const res = await fetch(`https://localhost:7005/api/items?${query}`, {
      credentials: "include",
    });

    if (res.ok) {
      const data = await res.json();
      setItems(data);
    }
  };

  const fetchCategories = async () => {
    const res = await fetch(`https://localhost:7005/api/categories`, {
      credentials: "include",
    });

    if (res.ok) {
      const data = await res.json();
      setCategories(data);
    }
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  useEffect(() => {
    fetchItems();
  }, [search, categoryId, sortBy, descending]);

  return (
    <div className="max-w-5xl mx-auto px-4 py-6">
      <h1 className="text-2xl font-bold mb-4">Explore Items</h1>

      <div className="flex flex-wrap gap-4 mb-6">
        <input
          type="text"
          placeholder="Search..."
          value={search}
          onChange={(e) => {
            setSearch(e.target.value);
          }}
          className="border px-3 py-2 rounded w-full sm:w-auto"
        />

        <select
          value={categoryId ?? ""}
          onChange={(e) => {
            const val = e.target.value;
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
            <li key={item.id} className="p-4 bg-white rounded shadow">
              <Link to={`/items/${item.id}`}>
                {item.title}
              </Link>
              <p className="text-sm text-gray-600">Category: {item.categoryName}</p>
              <p className="text-gray-800">Description: {item.description}</p>
              <p className="text-yellow-500">Rating: {item.averageRating}/5</p>
            </li>
          ))}
        </ul>
      )}
   </div>
  );
}