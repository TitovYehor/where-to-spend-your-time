import { Link } from "react-router-dom";
import { Tag, Boxes, FolderCog } from 'lucide-react';

export default function AdminDashboard() {
    return (
    <section aria-labelledby="admin-heading" className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 py-8 bg-white/60 backdrop-blur-md rounded-xl shadow-lg">
      <h1 id="admin-heading" className="text-3xl font-bold mb-6">Admin Panel</h1>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        <Link
          to="/admin/categories"
          className="block p-6 bg-white rounded-xl shadow hover:shadow-md transition-transform hover:scale-[1.02] text-center focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <div className="flex flex-col items-center justify-center">
            <FolderCog className="w-8 h-8 text-blue-600 mb-2" />
            <h2 className="text-xl font-semibold mb-1">Manage Categories</h2>
            <p className="text-sm text-gray-600">Create, update, or delete categories</p>
          </div>
        </Link>

        <Link
          to="/admin/items"
          className="block p-6 bg-white rounded-xl shadow hover:shadow-md transition-transform hover:scale-[1.02] text-center focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <div className="flex flex-col items-center justify-center">
            <Boxes className="w-8 h-8 text-blue-600 mb-2" />
            <h2 className="text-xl font-semibold mb-1">Manage Items</h2>
            <p className="text-sm text-gray-600">Create, update, or delete items</p>
          </div>
        </Link>

        <Link
          to="/admin/tags"
          className="block p-6 bg-white rounded-xl shadow hover:shadow-md transition-transform hover:scale-[1.02] text-center focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <div className="flex flex-col items-center justify-center">
            <Tag className="w-8 h-8 text-blue-600 mb-2" />
            <h2 className="text-xl font-semibold mb-1">Manage Tags</h2>
            <p className="text-sm text-gray-600">Create, update, or delete tags</p>
          </div>
        </Link>
      </div>
    </section>
  );
}