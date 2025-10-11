import { Link } from "react-router-dom";
import type { Tag } from "../../types/tag";
import { Tag as TagIcon } from "lucide-react";

type TagCardProps = {
  tag: Tag;
};

export default function TagCard({ tag }: TagCardProps) {
  return (
    <Link
      to={`/?tagsids=${tag.id}`}
      className="bg-white rounded-xl shadow p-4 hover:shadow-md transition-transform hover:scale-[1.02] focus:outline-none focus:ring-2 focus:ring-blue-500 flex items-center gap-2"
    >
      <TagIcon className="w-5 h-5 text-green-500" />
      <h3 className="text-lg font-semibold text-gray-800">{tag.name}</h3>
    </Link>
  );
}