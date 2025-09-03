export type Category = {
  id: number;
  name: string;  
};

export type CategoryCreateRequest = {
  name: string;
};

export type CategoryUpdateRequest = {
  name: string;
};