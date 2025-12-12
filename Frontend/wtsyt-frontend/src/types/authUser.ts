import type { Review } from "./review";
import type { Comment } from "./comment";
import type { UserRole } from "./userRole";

export type AuthUser = {
  id: string;
  displayName: string;
  email: string | null;
  role: UserRole;

  reviews: Review[];
  comments: Comment[];
};