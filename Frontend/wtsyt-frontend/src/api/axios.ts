import axios from "axios";
import { API_URL } from "./env";

const apiUrl = API_URL;

if (!apiUrl) {
  throw new Error("VITE_API_URL is not defined");
}

const api = axios.create({
  baseURL: `${apiUrl}/api`,
  withCredentials: true,
});

export default api;