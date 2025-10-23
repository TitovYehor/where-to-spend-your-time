import { Link } from "react-router-dom";
import type { Item } from "../../types/item";
import { Star, Folder, Tag } from "lucide-react";
import { formatRating } from "../../utils/formatters";

type ItemCardProps = {
  item: Item;
};

export default function ItemCard({ item }: ItemCardProps) {
  return (
    <Link
    to={`/items/${item.id}`}
    className="block p-4 bg-white rounded-lg shadow hover:shadow-md transition-transform 
        hover:scale-[1.01] focus:outline-none focus:ring-2 focus:ring-blue-500"
    >
      <h3 className="text-lg font-semibold text-gray-800">{item.title}</h3>
      <p className="flex items-center gap-1 text-sm text-blue-500">
        <Folder className="w-4 h-4" />
        {item.categoryName}
      </p>
      <p className="text-gray-700 whitespace-pre-line">Description: {item.description}</p>
      <p className="text-yellow-500 font-medium flex items-center gap-1">
        <Star className="w-5 h-5" />
        Rating: {formatRating(item.averageRating)}/5
      </p>

      {item.tags && item.tags.length > 0 && (
        <div className="mt-3 flex flex-wrap gap-2">
          {item.tags.map((tag) => (
            <span
              key={tag.id}
              className="flex items-center gap-1 bg-blue-100 text-green-700 text-xs font-medium px-3 py-1 rounded-full"
            >
              <Tag className="w-4 h-4" />
              {tag.name}
              </span>
          ))}
        </div>
      )}
    </Link>
  );
}