import type { Tag } from "./tag";
import type { Media } from "./media";

export type Item = {
  id: number;
  title: string;
  description: string;
  categoryId: number;
  categoryName: string;
  averageRating: number;
  tags: Tag[];
  media: Media[];
};

export type ItemCreateRequest = {
  title: string;
  description: string;
  categoryId: number;
};

export type ItemUpdateRequest = {
  title: string;
  description: string;
  categoryId: number;
};