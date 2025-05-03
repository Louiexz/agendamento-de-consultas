import { createRouter, createWebHistory } from 'vue-router';

//Todos os usuários
import usual_routes from "./usuarios.js";

//Admin
import admin_routes from "./admin.js";

//Admin e Paciente
import CadastrarConsulta from '@/views/Cadastro/CadastrarConsultaView.vue';

// Admin e Professor
import VisualizarPacientesView from '@/views/Visualizar/VisualizarPacientesView.vue';

// Professor
import TelaProfessor from '@/views/TelaProfessor.vue';

// Paciente
import TelaPaciente from '@/views/TelaPaciente.vue';

import { useAuthStore } from '@/store/auth';

const getAuth = () => useAuthStore();

const routes = [
  ...usual_routes,
  ...admin_routes,
  {
    path: '/cadastrar-consultas',
    name: 'cadastrar-consultas',
    component: CadastrarConsulta,
    meta: { requiresAuth: true, allowedRoles: ['Administrador', 'Paciente'] },
  },
  {
    path: '/pacientes',
    name: 'visualizar-pacientes',
    component: VisualizarPacientesView,
    meta: { requiresAuth: true, allowedRoles: ['Administrador', 'Professor'] },
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
  routes,
})

router.beforeEach((to, from, next) => {
  const auth = getAuth()

  if (auth.token && to.path === '/login') {
    if (auth.tipoUsuario === 'Administrador') {
      return next('/admin')
    } else if (auth.tipoUsuario === 'Professor') {
      return next('/professor')
    } else if (auth.tipoUsuario === 'Paciente') {
      return next('/paciente')
    }
  }

  if (to.meta.requiresAuth) {
    if (!auth.token) {
      return next('/login')
    }

    if (to.meta.allowedRoles && !to.meta.allowedRoles.includes(auth.tipoUsuario)) {
      return next('/login')
    }
  }

  next()
})


export default router
