import { Link } from "react-router-dom";
import type { Category } from "../../types/category";
import { Folder } from "lucide-react";

type CategoryCardProps = {
  category: Category;
};

export default function CategoryCard({ category }: CategoryCardProps) {
  return (
    <Link
      to={`/?categoryId=${category.id}`}
      className="bg-white rounded-xl shadow p-4 hover:shadow-md transition-transform hover:scale-[1.02] focus:outline-none focus:ring-2 focus:ring-blue-500 flex items-center gap-2"
    >
      <Folder className="w-5 h-5 text-blue-500" />
      <h3 className="text-lg font-semibold text-gray-800">{category.name}</h3>
    </Link>
  );
}