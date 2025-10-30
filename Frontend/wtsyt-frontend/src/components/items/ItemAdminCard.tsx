import { Folder, Pencil, Trash2 } from "lucide-react";
import type { Item } from "../../types/item";

interface ItemAdminProps {
  item: Item;
  onEdit: (item: Item) => void;
  onDelete: (id: number) => void;
}

const ItemAdminCard: React.FC<ItemAdminProps> = ({ item, onEdit, onDelete }) => (
  <li className="flex flex-col sm:flex-row justify-between items-center bg-gray-50 border rounded-xl p-4 shadow-sm">
    <div className="flex-1 text-left">
      <h3 className="font-semibold text-lg">{item.title}</h3>
      <p className="text-sm text-gray-600 mt-1 mb-2 whitespace-pre-wrap max-w-3xl line-clamp-3">{item.description}</p>
      <p className="flex items-center gap-1 text-sm text-blue-500">
        <Folder className="w-4 h-4" />
        {item.categoryName}
      </p>

      {item.tags && item.tags.length > 0 && (
        <div className="mt-3 flex flex-wrap gap-2">
          {item.tags.map((tag) => (
            <span
              key={tag.id}
              className="bg-blue-100 text-green-700 text-xs font-medium px-3 py-1 rounded-full"
            >
              {tag.name}
            </span>
          ))}
        </div>
      )}
    </div>

    <div className="mt-4 sm:mt-0 sm:ml-6 flex flex-col gap-3 items-center">
      <button
        onClick={() => onEdit(item)}
        className="text-blue-600 hover:text-blue-800 transition"
        aria-label={`Edit ${item.title}`}
      >
        <Pencil className="w-5 h-5" />
      </button>
      <button
        onClick={() => onDelete(item.id)}
        className="text-red-600 hover:text-red-800 transition"
        aria-label={`Delete ${item.title}`}
      >
        <Trash2 className="w-5 h-5" />
      </button>
    </div>
  </li>
);

export default ItemAdminCard;