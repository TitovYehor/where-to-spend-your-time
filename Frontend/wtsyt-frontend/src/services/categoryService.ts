import api from '../api/axios.ts';
import type { Category, CategoryCreateRequest, CategoryUpdateRequest } from '../types/category.ts';
import type { Item } from '../types/item.ts';

export const getCategories = async (): Promise<Category[]> => {
  const res = await api.get<Category[]>('/categories');
  return res.data;
};

export const getCategoryById = async (id: number): Promise<Category> => {
  const res = await api.get<Category>(`/categories/${id}`);
  return res.data;
};

export const getItemsInCategory = async (id: number): Promise<Item[]> => {
  const res = await api.get<Item[]>(`/categories/${id}/items`);
  return res.data;
};

export const addCategory = async (data: CategoryCreateRequest): Promise<Category> => {
  const res = await api.post<Category>(`/categories`, data);
  return res.data;
};

export const updateCategory = async (categoryId: number, data: CategoryUpdateRequest): Promise<void> => {
  await api.put(`/categories/${categoryId}`, data);
};

export const deleteCategory = async (categoryId: number): Promise<void> => {
  await api.delete(`/categories/${categoryId}`);
};