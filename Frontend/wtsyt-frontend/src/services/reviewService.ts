import api from '../api/axios.ts';
import { API_BASES } from '../api/endpoints.ts';
import type { ReviewPagedResult } from '../types/pagination/pagedResult.ts';
import type { Review, ReviewCreateRequest, ReviewUpdateRequest } from '../types/review.ts';

const itemReviews = (itemId: number) => `${API_BASES.items}/${itemId}${API_BASES.reviews}`;
const userReviews = (userId: string) => `${API_BASES.users}/${userId}${API_BASES.reviews}`;

export const buildReviewQuery = (params: {
  page?: number;
  pageSize?: number;
}): string => {
  const query = new URLSearchParams();

  if (params.page !== undefined) query.append("page", params.page.toString());
  if (params.pageSize !== undefined) query.append("pageSize", params.pageSize.toString());
  
  return query.toString();
};

export const getReviewsForItem = async (itemId: number, signal?: AbortSignal): Promise<Review[]> => {
  const res = await api.get<Review[]>(itemReviews(itemId), { signal });
  return res.data;
};

export const getPagedReviewsForItem = async (itemId: number, params: {
  page?: number;
  pageSize?: number;
}, signal?: AbortSignal): Promise<ReviewPagedResult> => {
  const query = buildReviewQuery(params);
  const res = await api.get<ReviewPagedResult>(`${itemReviews(itemId)}/paged?${query}`, { signal });
  return res.data;
};

export const getPagedReviewsForUser = async (userId: string, params: {
  page?: number;
  pageSize?: number;
}): Promise<ReviewPagedResult> => {
  const query = buildReviewQuery(params);
  const res = await api.get<ReviewPagedResult>(`${userReviews(userId)}/paged?${query}`);
  return res.data;
};

export const getMyReviewForItem = async (itemId: number, signal?: AbortSignal): Promise<Review> => {
  const res = await api.get<Review>(`${itemReviews(itemId)}/my`, { signal });
  return res.data;
};

export const getReviewById = async (reviewId: number, signal?: AbortSignal): Promise<Review> => {
  const res = await api.get<Review>(`${API_BASES.reviews}/${reviewId}`, { signal });
  return res.data;
};

export const addReview = async (data: ReviewCreateRequest): Promise<Review> => {
  const res = await api.post<Review>(`${API_BASES.reviews}`, data);
  return res.data;
};

export const updateReview = async (reviewId: number, data: ReviewUpdateRequest): Promise<void> => {
  await api.put(`${API_BASES.reviews}/${reviewId}`, data);
};

export const deleteReview = async (reviewId: number): Promise<void> => {
  await api.delete(`${API_BASES.reviews}/${reviewId}`);
};