import type { Review } from "./review";
import type { Comment } from "./comment";

export type AuthUser = {
  id: string;
  displayName: string;
  email: string | null;
  role: string | null;

  reviews: Review[];
  comments: Comment[];
};