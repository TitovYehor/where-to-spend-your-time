export type Review = {
  id: number;
  title: string;
  content: string;
  rating: number;
  createdAt: string;
  author: string;
  userId: string;
  itemTitle: string;
  itemId: number;
};

export type ReviewCreateRequest = {
  itemId: number;
  title: string;
  content: string;
  rating: number;
};

export type ReviewUpdateRequest = {
  title: string;
  content: string;
  rating: number;
};