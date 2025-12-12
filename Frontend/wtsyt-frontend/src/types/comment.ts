import type { UserRole } from "./userRole";

export type Comment = {
  id: number;
  content: string;
  createdAt: string;
  author: string;
  userId: string;
  authorRole: UserRole;
  reviewId: number;
};

export type CommentCreateRequest = {
  content: string;
};

export type CommentUpdateRequest = {
  content: string;
};