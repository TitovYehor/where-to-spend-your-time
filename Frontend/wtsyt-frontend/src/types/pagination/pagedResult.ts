import type { AuthUser } from "../authUser";
import type { Category } from "../category";
import type { Item } from "../item";
import type { Tag } from "../tag";

export interface PagedResult<T> {
    items: T[];
    totalCount: number;
};

export type ItemPagedResult = PagedResult<Item>;
export type CategoryPagedResult = PagedResult<Category>;
export type TagPagedResult = PagedResult<Tag>;
export type UserPagedResult = PagedResult<AuthUser>;