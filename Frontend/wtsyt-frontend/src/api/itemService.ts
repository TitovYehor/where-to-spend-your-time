import api from './axios.ts';

export interface ItemDto {
  id: number;
  title: string;
  description: string;
  categoryName: string;
  averageRating: number;
}

export const getItems = async (): Promise<ItemDto[]> => {
  console.log('Fetching items from API...');
  const res = await api.get<ItemDto[]>('/items');
  console.log('Received items:', res.data);
  return res.data;
};