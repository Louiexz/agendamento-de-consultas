// src/router/paciente.js
import TelaPaciente from '@/views/TelaPaciente.vue'

export default [
  {
    path: '/paciente',
    name: 'paciente',
    component: TelaPaciente,
    meta: { 
      requiresAuth: true,
      allowedRoles: ['Paciente'] 
    }
  }
]