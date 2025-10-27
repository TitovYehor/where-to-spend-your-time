import { Folder, Pencil, Trash2 } from "lucide-react";
import type { Category } from "../../types/category";

interface CategoryAdminProps {
  category: Category;
  onEdit: (category: Category) => void;
  onDelete: (id: number) => void;
}

const CategoryAdminCard: React.FC<CategoryAdminProps> = ({ category, onEdit, onDelete }) => (
  <li className="flex flex-col sm:flex-row justify-between items-center bg-gray-50 border rounded-xl p-4 shadow-sm">
    <div className="flex-1 flex items-center text-blue-600 gap-2">
      <Folder className="w-5 h-5" />
      <span className="text-lg font-medium">{category.name}</span>
    </div>
    
    <div className="mt-3 sm:mt-0 sm:ml-6 flex gap-4">
      <button
        onClick={() => onEdit(category)}
        className="flex items-center gap-1 text-blue-600 hover:underline font-medium"
      >
        <Pencil className="w-4 h-4" />
        Edit
      </button>
      <button
        onClick={() => onDelete(category.id)}
        className="flex items-center gap-1 text-red-600 hover:underline font-medium"
      >
        <Trash2 className="w-4 h-4" />
        Delete
      </button>
    </div>
  </li>
);

export default CategoryAdminCard;