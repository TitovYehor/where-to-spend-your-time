import type { Comment } from "../../types/comment";
import UserProfileLink from "../users/UserProfileLinks";

interface CommentItemProps {
  comment: Comment;
  canManage: boolean;
  onDelete: (commentId: number, commentAuthor: string) => void;
}

const CommentManagementCard: React.FC<CommentItemProps> = ({ comment, canManage, onDelete }) => (
  <li className="border rounded-lg p-3 bg-gray-50 shadow-sm">
    <div className="flex justify-between items-center mb-1">
      <p className="text-sm text-gray-600 flex items-center gap-1">
        <UserProfileLink userId={comment.userId} name={comment.author} />
        <span>• {new Date(comment.createdAt).toLocaleString()}</span>
      </p>
      {canManage && (
        <button
          onClick={() => onDelete(comment.id, comment.author)}
          className="text-sm text-white bg-red-600 hover:bg-red-700 px-3 py-1 rounded-md"
        >
          Delete
        </button>
      )}
    </div>
    <p className="text-gray-800 break-words">{comment.content}</p>
  </li>
);

export default CommentManagementCard;