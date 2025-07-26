import { Link } from "react-router-dom";

export default function AdminDashboard() {
  return (
    <div className="max-w-4xl mx-auto p-6">
      <h1 className="text-3xl font-bold mb-6">Admin Panel</h1>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <Link
          to="/admin/categories"
          className="block p-6 bg-white rounded-xl shadow hover:shadow-md transition text-center"
        >
          <h2 className="text-xl font-semibold mb-2">Manage Categories</h2>
          <p className="text-sm text-gray-600">Create, update, or delete categories</p>
        </Link>

        <Link
          to="/admin/items"
          className="block p-6 bg-white rounded-xl shadow hover:shadow-md transition text-center"
        >
          <h2 className="text-xl font-semibold mb-2">Manage Items</h2>
          <p className="text-sm text-gray-600">Create, update, or delete items</p>
        </Link>

        <Link
          to="/admin/tags"
          className="block p-6 bg-white rounded-xl shadow hover:shadow-md transition text-center"
        >
          <h2 className="text-xl font-semibold mb-2">Manage Tags</h2>
          <p className="text-sm text-gray-600">Create, update, or delete tags</p>
        </Link>
      </div>
    </div>
  );
}