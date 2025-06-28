import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7005/api',
  withCredentials: true,
});

export default api;