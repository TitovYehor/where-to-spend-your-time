export type Item = {
  id: number;
  title: string;
  description: string;
  categoryId: number;
  categoryName: string;
  averageRating: number;
};

export type ItemsResult = {
  items: Item[];
  totalCount: number; 
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