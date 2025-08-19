import { Link } from "react-router-dom";
import type { ItemStat } from "../../types/stats";

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
      <p className="text-sm text-gray-500">Category: {item.category}</p>
      <p className="text-yellow-500 font-medium">Rating: {item.averageRating}/5</p>
    </Link>
  );
}