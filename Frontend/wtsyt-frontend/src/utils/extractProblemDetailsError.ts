import { AxiosError } from "axios";

export function extractProblemDetailsError(
  error: unknown
): string | string[] {
  if (error instanceof AxiosError && error.response?.data) {
    const data = error.response.data;

    if (Array.isArray(data.errors) && data.errors.length > 0) {
      return data.errors;
    }

    if (typeof data.detail === "string" && data.detail.trim() !== "") {
      return data.detail;
    }

    if (typeof data.title === "string" && data.title.trim() !== "") {
      return data.title;
    }
  }

  return "An unexpected error occurred";
}