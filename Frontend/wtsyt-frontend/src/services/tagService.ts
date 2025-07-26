import api from '../api/axios.ts';
import type { Tag, TagCreateRequest, TagUpdateRequest } from '../types/tag.ts';

export const getTags = async (): Promise<Tag[]> => {
  const res = await api.get<Tag[]>('/tags');
  return res.data;
};

export const getTagById = async (id: number): Promise<Tag> => {
  const res = await api.get<Tag>(`/tags/${id}`);
  return res.data;
};

export const addTag = async (data: TagCreateRequest): Promise<Tag> => {
  const res = await api.post<Tag>(`/tags`, data);
  return res.data;
};

export const updateTag = async (tagId: number, data: TagUpdateRequest): Promise<void> => {
  await api.put(`/tags/${tagId}`, data);
};

export const deleteTag = async (tagId: number): Promise<void> => {
  await api.delete(`/tags/${tagId}`);
};