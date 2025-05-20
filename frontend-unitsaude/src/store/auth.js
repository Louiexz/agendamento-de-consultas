import { defineStore } from "pinia";
import Cookies from "js-cookie"; // Importa a biblioteca js-cookie

export const useAuthStore = defineStore("auth", {
  state: () => ({
    token: Cookies.get("token"), // Carrega o token do cookie, se existir
    tipoUsuario: Cookies.get("tipoUsuario"), // Carrega o tipo de usuário do cookie, se existir
    nomeUsuario: Cookies.get("nome"), // Carrega o nome do usuário do cookie, se existir
    id_Usuario: Cookies.get("id_Usuario"), // Adicione esta linha
  }),
  actions: {
    setToken(token) {
      this.token = token;
      Cookies.set("token", token, { expires: 0.1667 , secure: true, sameSite: 'strict' });
    },
    setTipoUsuario(tipo) {
      this.tipoUsuario = tipo;
      Cookies.set("tipoUsuario", tipo, { expires: 0.1667, secure: true, sameSite: 'strict' });
    },
    setNomeUsuario(nome) {
      this.nomeUsuario = nome;
      Cookies.set("nome", nome, { expires: 0.1667 , secure: true, sameSite: 'strict' });
    },
    setUserId(id) {
      // Adicione este método
      this.id_Usuario = id;
      Cookies.set("id_Usuario", id, { expires: 0.1667, secure: true, sameSite: 'strict' });
    },
    logout() {
      this.token = null;
      this.tipoUsuario = null;
      this.nomeUsuario = null;
      this.id_Usuario = null,
      Cookies.remove("token"); // Remove o token do cookie
      Cookies.remove("tipoUsuario"); // Remove o tipo de usuário do cookie
      Cookies.remove("nome"); // Remove o nome do usuário do cookie
      Cookies.remove('id_Usuario'); // Adicione esta linha
    },
    carregarToken() {
      // Método que pode ser chamado para garantir que o estado inicial
      // está sincronizado com os cookies
      this.token = Cookies.get("token");
      this.tipoUsuario = Cookies.get("tipoUsuario");
      this.nomeUsuario = Cookies.get("nome");
      this.id_Usuario = Cookies.get('id_Usuario'); // Adicione esta linha
    },
  },
});
