import api from '../api/axios.ts';
import type { AuthUser } from '../types/authUser.ts';
import type { ChangePasswordRequest, UpdateProfileRequest, UpdateUserRoleRequest } from '../types/profileRequests.ts';

export const getAllUsers = async (): Promise<AuthUser[]> => {
  const res = await api.get<AuthUser[]>('/users');
  return res.data;
};

export const getMyProfile = async (): Promise<AuthUser> => {
  const res = await api.get<AuthUser>('/users/me');
  return res.data;
};

export const getProfileById = async (id: string): Promise<AuthUser> => {
  const res = await api.get<AuthUser>(`/users/${id}`);
  return res.data;
};

export const getRoles = async (): Promise<string[]> => {
  const res = await api.get<string[]>('/users/roles');
  return res.data;
};

export const updateProfile = async (data: UpdateProfileRequest): Promise<void> => {
  await api.put(`/users/me`, data);
};

export const updatePassword = async (data: ChangePasswordRequest): Promise<void> => {
  await api.put(`/users/me/change-password`, data);
};

export const updateUserRole = async (userId: string, data: UpdateUserRoleRequest): Promise<void> => {
  await api.put(`/users/${userId}/role`, data);
};

export const deleteUser = async (userId: string): Promise<void> => {
  await api.delete(`/users/${userId}`);
};