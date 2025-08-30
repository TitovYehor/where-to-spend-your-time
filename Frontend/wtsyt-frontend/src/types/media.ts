export type MediaType = "Image" | "Video";

export type Media = {
  id: number;
  type: MediaType;
  url: string;
};

export type MediaUploadRequest = {
  itemId: number;
  type: MediaType;
  file: File;
};