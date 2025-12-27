import api from '../api/axios.ts';
import { API_BASES } from '../api/endpoints.ts';
import type { Comment, CommentCreateRequest, CommentUpdateRequest } from '../types/comment.ts';
import type { CommentPagedResult } from '../types/pagination/pagedResult.ts';

const reviewComments = (reviewId: number) => `${API_BASES.reviews}/${reviewId}${API_BASES.comments}`;
const userComments = (userId: string) => `${API_BASES.users}/${userId}${API_BASES.comments}`;

export const buildCommentQuery = (params: {
  page?: number;
  pageSize?: number;
}): string => {
  const query = new URLSearchParams();

  if (params.page !== undefined) query.append("page", params.page.toString());
  if (params.pageSize !== undefined) query.append("pageSize", params.pageSize.toString());
  
  return query.toString();
};

export const getCommentsForReview = async (reviewId: number, signal?: AbortSignal): Promise<Comment[]> => {
  const res = await api.get<Comment[]>(reviewComments(reviewId), { signal });
  return res.data;
};

export const getPagedCommentsForReview = async (reviewId: number, params: {
  page?: number;
  pageSize?: number;
}, signal?: AbortSignal): Promise<CommentPagedResult> => {
  const query = buildCommentQuery(params);
  const res = await api.get<CommentPagedResult>(`${reviewComments(reviewId)}/paged?${query}`, { signal });
  return res.data;
};

export const getPagedCommentsForUser = async (userId: string, params: {
  page?: number;
  pageSize?: number;
}): Promise<CommentPagedResult> => {
  const query = buildCommentQuery(params);
  const res = await api.get<CommentPagedResult>(`${userComments(userId)}/paged?${query}`);
  return res.data;
};

export const addComment = async (reviewId: number, data: CommentCreateRequest): Promise<Comment> => {
  const res = await api.post<Comment>(reviewComments(reviewId), data);
  return res.data;
};

export const updateComment = async (commentId: number, data: CommentUpdateRequest): Promise<void> => {
  await api.put(`${API_BASES.comments}/${commentId}`, data);
};

export const deleteComment = async (commentId: number): Promise<void> => {
  await api.delete(`${API_BASES.comments}/${commentId}`);
};