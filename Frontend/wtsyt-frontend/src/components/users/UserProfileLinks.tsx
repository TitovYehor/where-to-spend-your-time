import { Link } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";
import { Shield, User } from "lucide-react";

interface UserProfileLinkProps {
  userId: string;
  name: string;
  role: "User" | "Moderator" | "Admin";
  className?: string;
}

export default function UserProfileLink({ userId, name, role, className = "" }: UserProfileLinkProps) {
  const { user } = useAuth();

  const isCurrentUser = user && user.id === userId;
  const target = isCurrentUser ? "/profile" : `/users/${userId}`;

  const roleColor =
    role === "Admin" ? "text-red-600" :
    role === "Moderator" ? "text-yellow-700" :
    "text-gray-600";

  return (
    <Link 
      to={target} 
      className={`inline-flex items-center gap-1 text-violet-600 hover:underline ${className}`}
    >
      <User className="w-4 h-4 text-violet-500" />
      <Shield className={`w-4 h-4 ${roleColor}`} />
      <span className={`py-0.5 rounded-md text-xs font-semibold ${roleColor}`}>
        {role} |
      </span>
      <span>
        {name} {isCurrentUser && <span className="text-xs text-gray-500">(You)</span>}
      </span>
    </Link>
  );
}