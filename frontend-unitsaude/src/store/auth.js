import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: localStorage.getItem('token'), // Carrega o token de localStorage, se existir
    tipoUsuario: localStorage.getItem('tipoUsuario'), // Carrega o tipo de usuário de localStorage, se existir
    nomeUsuario: localStorage.getItem('nome'), // Carrega o nome do usuário de localStorage, se existir
  }),
  actions: {
    setToken(token) {
      this.token = token
      localStorage.setItem('token', token) // Salva no localStorage
    },
    setTipoUsuario(tipo) {
      this.tipoUsuario = tipo
      localStorage.setItem('tipoUsuario', tipo) // Salva no localStorage
    },
    setNomeUsuario(nome) {
      this.nomeUsuario = nome
      localStorage.setItem('nome', nome) // Salva no localStorage
    },
    logout() {
      this.token = null
      this.tipoUsuario = null
      localStorage.removeItem('token') // Remove o token do localStorage
      localStorage.removeItem('tipoUsuario') // Remove o tipo de usuário do localStorage
      localStorage.removeItem('nome') // Remove o nome do usuário do localStorage
    },
    carregarToken() {
      // Método que pode ser chamado para garantir que o estado inicial
      // está sincronizado com o localStorage
      this.token = localStorage.getItem('token')
      this.tipoUsuario = localStorage.getItem('tipoUsuario')
      this.nomeUsuario = localStorage.getItem('nome') // Carrega o nome do usuário
    },
  },
})
