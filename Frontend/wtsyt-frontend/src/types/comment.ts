export type Comment = {
  id: number;
  content: string;
  createdAt: string;
  author: string;
  userId: string;
  authorRole: "User" | "Moderator" | "Admin";
  reviewId: number;
};

export type CommentCreateRequest = {
  content: string;
};

export type CommentUpdateRequest = {
  content: string;
};