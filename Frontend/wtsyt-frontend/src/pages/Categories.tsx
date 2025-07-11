import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

type CategoryDto = {
  id: number;
  name: string;  
};

export default function Categories() {
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const res = await fetch(`https://localhost:7005/api/categories`, {
          credentials: "include",
        });

        if (res.ok) {
          const data = await res.json();
          setCategories(data);
        } else {
          console.error("Failed to fetch categories");
        }
      } catch (err) {
        console.error("Fetch error:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) return <p className="text-center mt-10">Loading...</p>;

  return (
    <div className="max-w-4xl mx-auto px-4">
      <h1 className="text-2xl font-bold mb-4">Categories</h1>
      <div className="grid gap-4">
        {categories.map(cat => (
          <Link
            key={cat.id}
            to={`/categories/${cat.id}`}
            className="bg-white rounded-xl shadow p-4 hover:shadow-md transition"
          >
            <h3 className="text-lg font-semibold">{cat.name}</h3>
          </Link>
        ))}
      </div>
    </div>
  );
}