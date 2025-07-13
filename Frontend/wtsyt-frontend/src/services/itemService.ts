import api from '../api/axios.ts';
import type { Item, ItemsResult, ItemCreateRequest, ItemUpdateRequest } from '../types/item.ts';

export const buildItemQuery = (params: {
  search?: string;
  categoryId?: number;
  sortBy?: string;
  descending?: boolean;
  page?: number;
  pageSize?: number;
}): string => {
  const query = new URLSearchParams();

  if (params.search) query.append("search", params.search);
  if (params.categoryId !== undefined) query.append("categoryId", params.categoryId.toString());
  if (params.sortBy) query.append("sortBy", params.sortBy);
  if (params.descending !== undefined) query.append("descending", params.descending.toString());
  if (params.page !== undefined) query.append("page", params.page.toString());
  if (params.pageSize !== undefined) query.append("pageSize", params.pageSize.toString());

  return query.toString();
};

export const getItems = async (params: {
  search?: string;
  categoryId?: number;
  sortBy?: string;
  descending?: boolean;
  page?: number;
  pageSize?: number;
}): Promise<ItemsResult> => {
  const query = buildItemQuery(params);
  const res = await api.get<ItemsResult>(`/items?${query}`);
  return res.data;
};

export const getItemById = async (id: number): Promise<Item> => {
  const res = await api.get<Item>(`/items/${id}`);
  return res.data;
};

export const addItem = async (data: ItemCreateRequest): Promise<Item> => {
  const res = await api.post<Item>(`/items`, data);
  return res.data;
};

export const updateItem = async (itemId: number, data: ItemUpdateRequest): Promise<void> => {
  await api.put(`/items/${itemId}`, data);
};

export const deleteItem = async (itemId: number): Promise<void> => {
  await api.delete(`/items/${itemId}`);
};