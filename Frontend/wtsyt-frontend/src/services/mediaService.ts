import api from '../api/axios.ts';
import { API_BASES } from '../api/endpoints.ts';
import type { Media, MediaUploadRequest } from '../types/media.ts';

export const uploadMedia = async (request: MediaUploadRequest): Promise<Media> => {  
  const formData = new FormData();
  formData.append('itemId', request.itemId.toString());
  formData.append('type', request.type);
  formData.append('file', request.file);

  const res = await api.post<Media>(`${API_BASES.media}`, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

  return res.data;
};

export const getMediaUrl = (url: string) => url;

export const deleteMedia = async (mediaId: number): Promise<boolean> => {
  const res = await api.delete<boolean>(`${API_BASES.media}/${mediaId}`);
  return res.data;
};