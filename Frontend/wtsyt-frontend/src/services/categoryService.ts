import api from '../api/axios.ts';
import { API_BASES } from '../api/endpoints.ts';
import type { Category, CategoryCreateRequest, CategoryUpdateRequest } from '../types/category.ts';
import type { Item } from '../types/item.ts';
import type { CategoryPagedResult } from '../types/pagination/pagedResult.ts';

export const buildCategoryQuery = (params: {
  search?: string;
  page?: number;
  pageSize?: number;
}): string => {
  const query = new URLSearchParams();

  if (params.search) query.append("search", params.search);
  if (params.page !== undefined) query.append("page", params.page.toString());
  if (params.pageSize !== undefined) query.append("pageSize", params.pageSize.toString());
  
  return query.toString();
};

export const getCategories = async (): Promise<Category[]> => {
  const res = await api.get<Category[]>(`${API_BASES.categories}`);
  return res.data;
};

export const getPagedCategories = async (params: {
  search?: string;
  page?: number;
  pageSize?: number;
}): Promise<CategoryPagedResult> => {
  const query = buildCategoryQuery(params);
  const res = await api.get<CategoryPagedResult>(`${API_BASES.categories}/paged?${query}`);
  return res.data;
};

export const getCategoryById = async (id: number): Promise<Category> => {
  const res = await api.get<Category>(`${API_BASES.categories}/${id}`);
  return res.data;
};

export const getItemsInCategory = async (id: number): Promise<Item[]> => {
  const res = await api.get<Item[]>(`${API_BASES.categories}/${id}/items`);
  return res.data;
};

export const addCategory = async (data: CategoryCreateRequest): Promise<Category> => {
  const res = await api.post<Category>(`${API_BASES.categories}`, data);
  return res.data;
};

export const updateCategory = async (categoryId: number, data: CategoryUpdateRequest): Promise<void> => {
  await api.put(`${API_BASES.categories}/${categoryId}`, data);
};

export const deleteCategory = async (categoryId: number): Promise<void> => {
  await api.delete(`${API_BASES.categories}/${categoryId}`);
};