import axios from 'axios';

// Definindo a URL base da API
const api = axios.create({
  baseURL: process.env.VUE_APP_API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Você pode configurar interceptores, autenticação ou outros cabeçalhos aqui, se necessário
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token'); // Ou do seu armazenamento preferido
    if (token) {
      config.headers['Authorization'] = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default api;
