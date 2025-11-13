import axios from "axios";

export const handleApiError = (error: unknown): string => {
  let userMessage = "An unexpected error occurred. Please try again";

  if (axios.isAxiosError(error)) {
    const status = error.response?.status;
    const responseData = error.response?.data;

    console.error("API Error:", {
      status,
      message: error.message,
      data: responseData,
      url: error.config?.url,
    });

    if (!error.response) {
      userMessage = "Network error — please check your internet connection";
    } else if (status === 400) {
      userMessage = "Invalid request. Please check your input";
    } else if (status === 401) {
      userMessage = "Unauthorized. Please log in again";
    } else if (status === 403) {
      userMessage = "You don't have permission to perform this action";
    } else if (status === 404) {
      userMessage = "Requested resource was not found";
    } else if (status && status >= 500) {
      userMessage = "Server error. Please try again later";
    }
  } else if (error instanceof Error) {
    console.error("Unexpected Error:", error.message);
    userMessage = error.message;
  } else {
    console.error("Unknown Error:", error);
  }

  return userMessage;
};