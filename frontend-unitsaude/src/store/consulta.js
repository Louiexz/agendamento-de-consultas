// store/consulta.js (com Pinia)
import { defineStore } from 'pinia'

export const useConsultaStore = defineStore('consulta', {
  state: () => ({ consulta: {} }),
  actions: {
    createStore() {
      // setup inicial, se precisar
    },
    setConsulta(consulta) {
      this.consulta = consulta
    },
    getConsulta() {
      return this.consulta;
    }
  }
})
