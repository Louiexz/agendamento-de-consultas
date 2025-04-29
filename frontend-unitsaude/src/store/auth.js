import { defineStore } from 'pinia';
import Cookies from 'js-cookie'; // Importa a biblioteca js-cookie

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: Cookies.get('token'), // Carrega o token do cookie, se existir
    tipoUsuario: Cookies.get('tipoUsuario'), // Carrega o tipo de usuário do cookie, se existir
    nomeUsuario: Cookies.get('nome'), // Carrega o nome do usuário do cookie, se existir
  }),
  actions: {
    setToken(token) {
      this.token = token;
      Cookies.set('token', token, { expires: 0.1667  }); // 4 horas
    },
    setTipoUsuario(tipo) {
      this.tipoUsuario = tipo;
      Cookies.set('tipoUsuario', tipo, { expires: 0.1667  }); // 4 horas
    },
    setNomeUsuario(nome) {
      this.nomeUsuario = nome;
      Cookies.set('nome', nome, { expires: 0.1667  }); // 4 horas
    },
    logout() {
      this.token = null;
      this.tipoUsuario = null;
      this.nomeUsuario = null;
      Cookies.remove('token'); // Remove o token do cookie
      Cookies.remove('tipoUsuario'); // Remove o tipo de usuário do cookie
      Cookies.remove('nome'); // Remove o nome do usuário do cookie
    },
    carregarToken() {
      // Método que pode ser chamado para garantir que o estado inicial
      // está sincronizado com os cookies
      this.token = Cookies.get('token');
      this.tipoUsuario = Cookies.get('tipoUsuario');
      this.nomeUsuario = Cookies.get('nome');
    },
  },
});
