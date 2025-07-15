import axios from "axios";

export const handleApiError = (error: unknown) => {
  if (axios.isAxiosError(error)) {
    console.error('API error:', error.response?.data || error.message);
  } else {
    console.error('Unexpected error:', error);
  }
};