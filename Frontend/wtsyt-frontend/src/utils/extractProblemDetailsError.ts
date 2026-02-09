import { AxiosError } from "axios";

export function extractProblemDetailsError(error: unknown): string {
  if (error instanceof AxiosError && error.response?.data) {
    const data = error.response.data;

    if (Array.isArray(data.errors)) {
      return data.errors.join(", ");
    }

    if (typeof data.detail === "string") {
      return data.detail;
    }

    if (typeof data.title === "string") {
      return data.title;
    }
  }

  return "An unexpected error occurred";
}