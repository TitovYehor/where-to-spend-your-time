import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';

type CategoryDto = {
  id: number;
  name: string;  
};

export default function Categories() {
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchData = async() => {
    const res = await fetch(`https://localhost:7005/api/categories`, {
        credentials: "include",
    });

    if (res.ok) {
        const data = await res.json();
        setCategories(data);
    }

    setLoading(false);
  };


  useEffect(() => {
    fetchData();
  });

  if (loading) return <p className="text-center mt-10">Loading...</p>;

  return (
    <div className="max-w-4xl mx-auto px-4">
      <h1 className="text-2xl font-bold mb-4">Categories</h1>
      <div className="grid gap-4">
        {categories.map(cat => (
          <div
            key={cat.id}
            className="bg-white rounded-xl shadow p-4 hover:shadow-md transition"
          >
            <p className="text-gray-600">{cat.name}</p>
            <Link to={`/categories/${cat.id}`}>
              Items in '{cat.name}' category
            </Link>
          </div>
        ))}
      </div>
    </div>
  );
}