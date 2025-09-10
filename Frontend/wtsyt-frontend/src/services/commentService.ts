import api from '../api/axios.ts';
import type { Comment, CommentCreateRequest, CommentUpdateRequest } from '../types/comment.ts';
import type { CommentPagedResult } from '../types/pagination/pagedResult.ts';

export const buildCommentQuery = (params: {
  page?: number;
  pageSize?: number;
}): string => {
  const query = new URLSearchParams();

  if (params.page !== undefined) query.append("page", params.page.toString());
  if (params.pageSize !== undefined) query.append("pageSize", params.pageSize.toString());
  
  return query.toString();
};

export const getCommentsForReview = async (reviewId: number): Promise<Comment[]> => {
  const res = await api.get<Comment[]>(`/reviews/${reviewId}/comments`);
  return res.data;
};

export const getPagedCommentsForReview = async (reviewId: number, params: {
  page?: number;
  pageSize?: number;
}): Promise<CommentPagedResult> => {
  const query = buildCommentQuery(params);
  const res = await api.get<CommentPagedResult>(`/reviews/${reviewId}/comments/paged?${query}`);
  return res.data;
};

export const getPagedCommentsForUser = async (userId: string, params: {
  page?: number;
  pageSize?: number;
}): Promise<CommentPagedResult> => {
  const query = buildCommentQuery(params);
  const res = await api.get<CommentPagedResult>(`/users/${userId}/comments/paged?${query}`);
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