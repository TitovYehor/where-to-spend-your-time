import { Pencil, Trash2 } from "lucide-react";
import type { AuthUser } from "../../types/authUser";
import UserProfileLink from "./UserProfileLinks";

interface UserAdminProps {
  user: AuthUser | null;
  displayUser: AuthUser;
  onEdit: (user: AuthUser) => void;
  onDelete: (id: string) => void;
}

const UserAdminCard: React.FC<UserAdminProps> = ({ user, displayUser, onEdit, onDelete }) => (
  <li className="flex flex-col sm:flex-row justify-between items-center bg-gray-50 border rounded-xl p-4 shadow-sm">
    <div className="flex-1">
      <p className="text-lg font-semibold text-gray-900">
        <UserProfileLink
          userId={displayUser.id}
          name={displayUser.displayName}
          role={displayUser.role}
          className="text-lg font-semibold"
        />
      </p>
    </div>

    <div className="mt-2 sm:mt-0">
      <span
        className={`inline-block px-3 py-1 text-sm font-medium rounded-full ${
          displayUser.role === "Admin"
          ? "bg-red-100 text-red-700"
          : displayUser.role === "Moderator"
          ? "bg-yellow-100 text-yellow-700"
          : "bg-blue-100 text-blue-700"
        }`}
      >
        {displayUser.role}
      </span>
    </div>

    {user?.id != displayUser.id && (
      <div className="mt-3 sm:mt-0 sm:ml-6 flex gap-4">
        <button
          onClick={() => onEdit(displayUser)}
          className="text-blue-600 hover:underline font-medium"
        >
          <Pencil className="w-5 h-5" />
        </button>
        <button
          onClick={() => onDelete(displayUser.id)}
          className="text-red-600 hover:underline font-medium"
        >
          <Trash2 className="w-5 h-5" />
        </button>
      </div>
    )}
  </li>
);

export default UserAdminCard;