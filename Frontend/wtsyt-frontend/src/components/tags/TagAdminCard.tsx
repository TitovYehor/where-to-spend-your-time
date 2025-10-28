import { Pencil, TagIcon, Trash2 } from "lucide-react";
import type { Tag } from "../../types/tag";

interface TagAdminProps {
  tag: Tag;
  onEdit: (Tag: Tag) => void;
  onDelete: (id: number) => void;
}

const TagAdminCard: React.FC<TagAdminProps> = ({ tag, onEdit, onDelete }) => (
  <li className="flex flex-col sm:flex-row justify-between items-center bg-gray-50 border rounded-xl p-4 shadow-sm">
    <div className="flex-1 flex items-center text-green-700 gap-2">
      <TagIcon className="w-5 h-5" />
      <span className="text-lg font-medium">{tag.name}</span>
    </div>

    <div className="mt-3 sm:mt-0 sm:ml-6 flex gap-4">
      <button
        onClick={() => onEdit(tag)}
        className="flex items-center gap-1 text-blue-600 hover:underline font-medium"
      >
        <Pencil className="w-4 h-4" />
        Edit
      </button>
      <button
        onClick={() => onDelete(tag.id)}
        className="flex items-center gap-1 text-red-600 hover:underline font-medium"
      >
        <Trash2 className="w-4 h-4" />
        Delete
      </button>
    </div>
  </li>
);

export default TagAdminCard;