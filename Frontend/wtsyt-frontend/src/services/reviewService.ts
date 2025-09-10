import api from '../api/axios.ts';
import type { ReviewPagedResult } from '../types/pagination/pagedResult.ts';
import type { Review, ReviewCreateRequest, ReviewUpdateRequest } from '../types/review.ts';

export const buildReviewQuery = (params: {
  page?: number;
  pageSize?: number;
}): string => {
  const query = new URLSearchParams();

  if (params.page !== undefined) query.append("page", params.page.toString());
  if (params.pageSize !== undefined) query.append("pageSize", params.pageSize.toString());
  
  return query.toString();
};

export const getReviewsForItem = async (itemId: number): Promise<Review[]> => {
  const res = await api.get<Review[]>(`/items/${itemId}/reviews`);
  return res.data;
};

export const getPagedReviewsForItem = async (itemId: number, params: {
  page?: number;
  pageSize?: number;
}): Promise<ReviewPagedResult> => {
  const query = buildReviewQuery(params);
  const res = await api.get<ReviewPagedResult>(`/items/${itemId}/reviews/paged?${query}`);
  return res.data;
};

export const getPagedReviewsForUser = async (userId: string, params: {
  page?: number;
  pageSize?: number;
}): Promise<ReviewPagedResult> => {
  const query = buildReviewQuery(params);
  const res = await api.get<ReviewPagedResult>(`/users/${userId}/reviews/paged?${query}`);
  return res.data;
};

export const getMyReviewForItem = async (itemId: number): Promise<Review> => {
  const res = await api.get<Review>(`/items/${itemId}/reviews/my`);
  return res.data;
};

export const getReviewById = async (reviewId: number): Promise<Review> => {
  const res = await api.get<Review>(`/reviews/${reviewId}`);
  return res.data;
};

export const addReview = async (data: ReviewCreateRequest): Promise<Review> => {
  const res = await api.post<Review>(`/reviews`, data);
  return res.data;
};

export const updateReview = async (reviewId: number, data: ReviewUpdateRequest): Promise<void> => {
  await api.put(`/reviews/${reviewId}`, data);
};

export const deleteReview = async (reviewId: number): Promise<void> => {
  await api.delete(`/reviews/${reviewId}`);
};