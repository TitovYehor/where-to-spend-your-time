import api from './axios.ts';

export interface ItemDto {
  id: number;
  title: string;
  description: string;
  categoryName: string;
  averageRating: number;
}

export const getItems = async (): Promise<ItemDto[]> => {
  const res = await api.get<ItemDto[]>('/items');
  return res.data;
};