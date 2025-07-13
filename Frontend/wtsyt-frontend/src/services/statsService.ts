import api from '../api/axios.ts';
import type { Stats } from '../types/stats.ts';

export const getStats = async (): Promise<Stats> => {
  const res = await api.get<Stats>('/stats');
  return res.data;
};