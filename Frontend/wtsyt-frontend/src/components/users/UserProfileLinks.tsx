import { Link } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";
import { User } from "lucide-react";

interface UserProfileLinkProps {
  userId: string;
  name: string;
  className?: string;
}

export default function UserProfileLink({ userId, name, className = "" }: UserProfileLinkProps) {
  const { user } = useAuth();

  const isCurrentUser = user && user.id === userId;
  const target = isCurrentUser ? "/profile" : `/users/${userId}`;

  return (
    <Link 
      to={target} 
      className={`inline-flex items-center gap-1 text-blue-600 hover:underline ${className}`}
    >
      <User className="w-4 h-4 text-blue-500" />
      <span>
        {name} {isCurrentUser && <span className="text-xs text-gray-500">(You)</span>}
      </span>
    </Link>
  );
}