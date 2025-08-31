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
  return `https://localhost:7005${relativeUrl}`;
};

export const deleteMedia = async (mediaId: number): Promise<boolean> => {
  const res = await api.delete<boolean>(`/media/${mediaId}`);
  return res.data;
};