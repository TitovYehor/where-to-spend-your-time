import api from '../api/axios.ts';
import type { Media, MediaUploadRequest } from '../types/media.ts';

export const uploadMedia = async (request: MediaUploadRequest): Promise<Media> => {  
  const formData = new FormData();
  formData.append('itemId', request.itemId.toString());
  formData.append('type', request.type);
  formData.append('file', request.file);

  const res = await api.post<Media>('/media', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

  return res.data;
};

export const getMediaUrl = (relativeUrl: string) => {
  const apiUrl = import.meta.env.VITE_API_URL;
  return `${apiUrl}${relativeUrl}`;
};

export const deleteMedia = async (mediaId: number): Promise<boolean> => {
  const res = await api.delete<boolean>(`/media/${mediaId}`);
  return res.data;
};