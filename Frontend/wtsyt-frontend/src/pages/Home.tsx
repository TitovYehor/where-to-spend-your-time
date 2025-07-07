import { useEffect, useState } from "react";

type ItemDto = {
  id: number;
  title: string;
  description: string;
  categoryName: string;
  averageRating: number;
};

export default function Home() {
  const [items, setItems] = useState<ItemDto[]>([]);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(false);

  const fetchItems = async () => {
    setLoading(true);
    const query = new URLSearchParams({
      search: search.trim(),
      page: "1",
      pageSize: "20",
    });

    const res = await fetch(`https://localhost:7005/api/items?${query}`, {
      credentials: "include",
    });

    if (res.ok) {
      const data = await res.json();
      setItems(data);
    }
    setLoading(false);
  };

  useEffect(() => {
    fetchItems();
  }, []);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    fetchItems();
  };

  return (
    <div className="max-w-4xl mx-auto p-4">
      <form onSubmit={handleSearch} className="mb-6">
        <input
          type="text"
          placeholder="Search items..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="border p-2 rounded w-full"
        />
      </form>

      {loading ? (
        <p>Loading items...</p>
      ) : (
        <div className="grid gap-4">
          {items.map((item) => (
            <div key={item.id} className="bg-white p-4 rounded shadow">
              <h2 className="text-lg font-semibold">{item.title}</h2>
              <p className="text-gray-600">{item.description}</p>
              <p className="text-sm text-gray-500">
                Category: {item.categoryName} | Rating: {item.averageRating}/5
              </p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}