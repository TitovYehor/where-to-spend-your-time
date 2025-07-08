import api from './axios.ts';

export interface ItemDto {
  id: number;
  title: string;
  description: string;
  categoryName: string;
  averageRating: number;
}

export interface ItemsResult
{
  items: ItemDto[];
  totalCount: number; 
}

export const getItems = async (): Promise<ItemsResult> => {
  const res = await api.get<ItemsResult>('/items');
  return res.data;
};