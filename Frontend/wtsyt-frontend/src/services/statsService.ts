import api from '../api/axios.ts';
import { API_BASES } from '../api/endpoints.ts';
import type { Stats } from '../types/stats.ts';

export const getStats = async (): Promise<Stats> => {
  const res = await api.get<Stats>(`${API_BASES.stats}`);
  return res.data;
};