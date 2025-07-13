import api from '../api/axios.ts';
import type { LoginRequest, RegisterRequest } from '../types/authRequests.ts';

export const login = async (data: LoginRequest): Promise<void> => {
  await api.post(`/auth/login`, data);
};

export const register = async (data: RegisterRequest): Promise<void> => {
  await api.post(`/auth/register`, data);
};

export const logout = async (): Promise<void> => {
  await api.post(`/auth/logout`);
};