import { defineStore } from "pinia";
import Cookies from "js-cookie";
import { jwtDecode } from "jwt-decode";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    token: Cookies.get("token") || null,
    tipoUsuario: null,
    nomeUsuario: null,
    id_Usuario: null,
  }),
  actions: {
    setToken(token) {
      this.token = token;
      Cookies.set("token", token, { expires: 0.1667, secure: true, sameSite: 'strict' });

      try {
        const decoded = jwtDecode(token);
        console.log(decoded);
        this.tipoUsuario = decoded.role;       // ou "TipoUsuario", dependendo do claim
        this.nomeUsuario = decoded.unique_name;
        this.id_Usuario = decoded.nameid;      // ou "Id_Usuario", dependendo do claim
      } catch (error) {
        console.error("Token inválido");
        this.logout();
      }
    },
    getNomeUsuario() {
      return this.nomeUsuario;
    },
    getTipoUsuario() {
      return this.tipoUsuario;
    },
    decodeToken(token) {
      try {
        const decoded = jwtDecode(token);
        this.tipoUsuario = decoded.role || null; // Ajuste o campo conforme o nome do claim no seu JWT
        this.nomeUsuario = decoded.name || null;
        this.id_Usuario = decoded.nameid || null;
      } catch (error) {
        this.tipoUsuario = null;
        this.nomeUsuario = null;
        this.id_Usuario = null;
      }
    },
    logout() {
      this.token = null;
      this.tipoUsuario = null;
      this.nomeUsuario = null;
      this.id_Usuario = null;
      Cookies.remove("token");
    },
    carregarToken() {
      const token = Cookies.get("token");
      this.token = token || null;
      if (token) {
        this.decodeToken(token);
      }
    }
  },
});
