import type { AuthUser } from "../authUser";
import type { Category } from "../category";
import type { Item } from "../item";
import type { Review } from "../review";
import type { Tag } from "../tag";
import type { Comment } from "../comment";

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
};

export type ItemPagedResult = PagedResult<Item>;
export type CategoryPagedResult = PagedResult<Category>;
export type TagPagedResult = PagedResult<Tag>;
export type UserPagedResult = PagedResult<AuthUser>;
export type ReviewPagedResult = PagedResult<Review>;
export type CommentPagedResult = PagedResult<Comment>;