import api from '../api/axios.ts';
import { API_BASES } from '../api/endpoints.ts';
import type { TagPagedResult } from '../types/pagination/pagedResult.ts';
import type { Tag, TagCreateRequest, TagUpdateRequest } from '../types/tag.ts';

export const buildTagQuery = (params: {
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

export const getTags = async (signal?: AbortSignal): Promise<Tag[]> => {
  const res = await api.get<Tag[]>(`${API_BASES.tags}`, { signal });
  return res.data;
};

export const getPagedTags = async (params: {
  search?: string;
  page?: number;
  pageSize?: number;
}, signal?: AbortSignal): Promise<TagPagedResult> => {
  const query = buildTagQuery(params);
  const res = await api.get<TagPagedResult>(`${API_BASES.tags}/paged?${query}`, { signal });
  return res.data;
};

export const getTagById = async (id: number): Promise<Tag> => {
  const res = await api.get<Tag>(`${API_BASES.tags}/${id}`);
  return res.data;
};

export const addTag = async (data: TagCreateRequest): Promise<Tag> => {
  const res = await api.post<Tag>(`${API_BASES.tags}`, data);
  return res.data;
};

export const updateTag = async (tagId: number, data: TagUpdateRequest): Promise<void> => {
  await api.put(`${API_BASES.tags}/${tagId}`, data);
};

export const deleteTag = async (tagId: number): Promise<void> => {
  await api.delete(`${API_BASES.tags}/${tagId}`);
};