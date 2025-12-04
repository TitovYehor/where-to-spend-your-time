import type { Review } from "./review";

export type ItemStat = {
  id: number;
  title: string;
  category: string;
  averageRating: number;
  reviewCount: number;
};

type UserStat = {
  userId: string;
  displayName: string;
  reviewCount: number;
  role: "User" | "Moderator" | "Admin";
};

export type Stats = {
  topRatedItems: ItemStat[];
  mostReviewedItems: ItemStat[];
  topReviewers: UserStat[];
  recentReviews: Review[];
};