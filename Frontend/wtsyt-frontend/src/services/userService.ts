import api from '../api/axios.ts';
import { API_BASES } from '../api/endpoints.ts';
import type { AuthUser } from '../types/authUser.ts';
import type { UserPagedResult } from '../types/pagination/pagedResult.ts';
import type { ChangePasswordRequest, UpdateProfileRequest, UpdateUserRoleRequest } from '../types/profileRequests.ts';

export const buildUserQuery = (params: {
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

export const getAllUsers = async (): Promise<AuthUser[]> => {
  const res = await api.get<AuthUser[]>(`${API_BASES.users}`);
  return res.data;
};

export const getPagedUsers = async (params: {
  search?: string;
  page?: number;
  pageSize?: number;
}): Promise<UserPagedResult> => {
  const query = buildUserQuery(params);
  const res = await api.get<UserPagedResult>(`${API_BASES.users}/paged?${query}`);
  return res.data;
};

export const getMyProfile = async (): Promise<AuthUser> => {
  const res = await api.get<AuthUser>(`${API_BASES.users}/me`);
  return res.data;
};

export const getProfileById = async (id: string): Promise<AuthUser> => {
  const res = await api.get<AuthUser>(`${API_BASES.users}/${id}`);
  return res.data;
};

export const getRoles = async (): Promise<string[]> => {
  const res = await api.get<string[]>(`${API_BASES.users}/roles`);
  return res.data;
};

export const updateProfile = async (data: UpdateProfileRequest): Promise<void> => {
  await api.put(`${API_BASES.users}/me`, data);
};

export const updatePassword = async (data: ChangePasswordRequest): Promise<void> => {
  await api.put(`${API_BASES.users}/me/change-password`, data);
};

export const updateUserRole = async (userId: string, data: UpdateUserRoleRequest): Promise<void> => {
  await api.put(`${API_BASES.users}/${userId}/role`, data);
};

export const deleteUser = async (userId: string): Promise<void> => {
  await api.delete(`${API_BASES.users}/${userId}`);
};