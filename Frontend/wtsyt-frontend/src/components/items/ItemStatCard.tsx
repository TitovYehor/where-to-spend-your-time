import { Link } from "react-router-dom";
import type { ItemStat } from "../../types/stats";
import { Star, Folder } from "lucide-react";

type ItemStatCardProps = {
  item: ItemStat;
};

export default function ItemStatCard({ item }: ItemStatCardProps) {
  return (
    <Link
      to={`/items/${item.id}`}
      className="block border border-gray-100 rounded-xl p-4 bg-white shadow-sm hover:shadow-md transition"
    >
      <h3 className="text-lg font-semibold">{item.title}</h3>

      <div className="space-y-1 text-sm text-gray-600">
        <p className="flex items-center gap-2">
          <Folder className="w-4 h-4 text-gray-400" />
          {item.category}
        </p>
        <p className="flex items-center gap-2 text-yellow-600 font-medium">
          <Star className="w-4 h-4" />
          {item.averageRating}/5
        </p>
      </div>
    </Link>
  );
}