import axios from 'axios';
import Cookies from 'js-cookie'; // Importa a biblioteca js-cookie

// Definindo a URL base da API
const api = axios.create({
  baseURL: import.meta.env.VITE_APP_API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Configurando interceptores para adicionar o token nos cabeçalhos da requisição
api.interceptors.request.use(
  (config) => {
    const token = Cookies.get('token'); // Recupera o token do cookie
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
