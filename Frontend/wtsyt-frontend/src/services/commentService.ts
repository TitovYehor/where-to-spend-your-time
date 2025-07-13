import api from '../api/axios.ts';
import type { Comment, CommentCreateRequest, CommentUpdateRequest } from '../types/comment.ts';

export const getCommentsForReview = async (reviewId: number): Promise<Comment[]> => {
  const res = await api.get<Comment[]>(`/reviews/${reviewId}/comments`);
  return res.data;
};

export const addComment = async (reviewId: number, data: CommentCreateRequest): Promise<Comment> => {
  const res = await api.post<Comment>(`/reviews/${reviewId}/comments`, data);
  return res.data;
};

export const updateComment = async (commentId: number, data: CommentUpdateRequest): Promise<void> => {
  await api.put(`/comments/${commentId}`, data);
};

export const deleteComment = async (commentId: number): Promise<void> => {
  await api.delete(`/comments/${commentId}`);
};