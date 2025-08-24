import { Link } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";

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
    <Link to={target} className={`text-blue-600 hover:underline ${className}`}>
      {name} {isCurrentUser && <span className="text-xs text-gray-500">(You)</span>}
    </Link>
  );
}