// store/usuario.js (com Pinia)
import { defineStore } from 'pinia'

export const useUsuarioStore = defineStore('usuario', {
  state: () => ({
    perfilAtual: null | 'paciente' | 'professor',
    usuario: {}
  }),
  actions: {
    createStore() {
      // setup inicial, se precisar
    },
    setUsuario(usuario) {
      this.usuario = usuario
    },
    getUsuario() {
      return this.usuario;
    },
    setPerfil(perfil= 'paciente' | 'professor') {
      this.perfilAtual = perfil
    },
    limparPerfil() {
      this.perfilAtual = null
    }
  },

  persist: true
})
