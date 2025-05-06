// store/usuario.js (com Pinia)
import { defineStore } from 'pinia'

export const useUsuarioStore = defineStore('usuario', {
  state: () => ({ usuario: {} }),
  actions: {
    createStore() {
      // setup inicial, se precisar
    },
    setUsuario(usuario) {
      this.usuario = usuario
    },
    getUsuario() {
      return this.usuario;
    }
  },

  persist: true
})
