import api from '../api/axios.ts';
import { API_BASES } from '../api/endpoints.ts';
import type { Item, ItemCreateRequest, ItemUpdateRequest } from '../types/item.ts';
import type { ItemPagedResult } from '../types/pagination/pagedResult.ts';
import type { Tag, TagRequest } from '../types/tag.ts';

export const buildItemQuery = (params: {
  search?: string;
  categoryId?: number;
  tagsids?: number[];
  sortBy?: string;
  descending?: boolean;
  page?: number;
  pageSize?: number;
}): string => {
  const query = new URLSearchParams();

  if (params.search) query.append("search", params.search);
  if (params.categoryId !== undefined) query.append("categoryId", params.categoryId.toString());
  
  params.tagsids?.forEach(id => query.append("tagsids", id.toString()));
  
  if (params.sortBy) query.append("sortBy", params.sortBy);
  if (params.descending !== undefined) query.append("descending", params.descending.toString());
  if (params.page !== undefined) query.append("page", params.page.toString());
  if (params.pageSize !== undefined) query.append("pageSize", params.pageSize.toString());
  
  return query.toString();
};

export const getItems = async (params: {
  search?: string;
  categoryId?: number;
  tagsids?: number[];
  sortBy?: string;
  descending?: boolean;
  page?: number;
  pageSize?: number;
}): Promise<ItemPagedResult> => {
  const query = buildItemQuery(params);
  const res = await api.get<ItemPagedResult>(`${API_BASES.items}?${query}`);
  return res.data;
};

export const getItemById = async (id: number): Promise<Item> => {
  const res = await api.get<Item>(`${API_BASES.items}/${id}`);
  return res.data;
};

export const addItem = async (data: ItemCreateRequest): Promise<Item> => {
  const res = await api.post<Item>(`${API_BASES.items}`, data);
  return res.data;
};

export const updateItem = async (itemId: number, data: ItemUpdateRequest): Promise<void> => {
  await api.put(`${API_BASES.items}/${itemId}`, data);
};

export const deleteItem = async (itemId: number): Promise<void> => {
  await api.delete(`${API_BASES.items}/${itemId}`);
};

export const addTagForItem = async (itemId: number, tag: TagRequest): Promise<Tag> => {
  const res = await api.post(`${API_BASES.items}/${itemId}${API_BASES.tags}`, tag);
  return res.data;
};

export const removeTagFromItem = async (itemId: number, tagName: string): Promise<void> => {
  await api.delete(`${API_BASES.items}/${itemId}${API_BASES.tags}/remove/${tagName}`);
};