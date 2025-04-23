import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '@/views/LoginView.vue'
import TelaAdmin from '@/views/TelaAdmin.vue'
import TelaProfessor from '@/views/TelaProfessor.vue'
import TelaPaciente from '@/views/TelaPaciente.vue'
import CadastroPaciente from '@/views/CadastroPacienteView.vue'
import RecuperarSenha from '@/views/RecuperarSenhaView.vue'
import RedefinirSenha from '@/views/RedefinirSenhaView.vue'

import { useAuthStore } from '@/store/auth'
import { storeToRefs } from 'pinia'
import { createPinia } from 'pinia'

const pinia = createPinia()

const routes = [
  {
    path: '/',
    name: 'Login',
    component: LoginView,
  },
  {
    path: '/cadastroPaciente',
    name: 'CadastroPaciente',
    component: CadastroPaciente,
  },
  {
    path: '/recuperarSenha',
    name: 'RecuperarSenha',
    component: RecuperarSenha,
  },
  {
    path: '/redefinirSenha',
    name: 'RedefinirSenha',
    component: RedefinirSenha,
  },
  {
    path: '/admin',
    name: 'admin',
    component: TelaAdmin,
    meta: { requiresAuth: true, allowedRoles: ['Administrador'] },
  },
  {
    path: '/professor',
    name: 'professor',
    component: TelaProfessor,
    meta: { requiresAuth: true, allowedRoles: ['Professor'] },
  },
  {
    path: '/paciente',
    name: 'paciente',
    component: TelaPaciente,
    meta: { requiresAuth: true, allowedRoles: ['Paciente'] },
  },
]


const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

router.beforeEach((to, from, next) => {
  const auth = useAuthStore(pinia)

  if (to.meta.requiresAuth) {
    if (!auth.token) {
      return next('/')
    }

    if (to.meta.allowedRoles && !to.meta.allowedRoles.includes(auth.tipoUsuario)) {
      return next('/')
    }
  }

  next()
})

export default router
