import type { Review } from "./review";
import type { Comment } from "./comment";

export type AuthUser = {
  id: string;
  displayName: string;
  email: string | null;
  role: "User" | "Moderator" | "Admin";

  reviews: Review[];
  comments: Comment[];
};