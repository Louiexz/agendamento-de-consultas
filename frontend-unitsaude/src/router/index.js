import { createRouter, createWebHistory } from 'vue-router';
import LoginView from '@/views/LoginView.vue';
import TelaAdmin from '@/views/TelaAdmin.vue';
import TelaProfessor from '@/views/TelaProfessor.vue';
import TelaPaciente from '@/views/TelaPaciente.vue';
import CadastroPaciente from '@/views/CadastroPacienteView.vue';
import CadastroProfessor from '@/views/CadastroProfessorView.vue';
import Registrarse from '@/views/RegistrarseView.vue';
import RecuperarSenha from '@/views/RecuperarSenhaView.vue';
import RedefinirSenha from '@/views/RedefinirSenhaView.vue';
import DisponibilizarHorarios from '@/views/DisponibilizarHorariosView.vue';
import CadastrarConsulta from '@/views/CadastrarConsultaView.vue';
import VisualizarPacientesView from '@/views/VisualizarPacientesView.vue';
import VisualizarProfessoresView from '@/views/VisualizarProfessoresView.vue';

import { useAuthStore } from '@/store/auth';

const getAuth = () => useAuthStore();

const routes = [

  {
    path: '/Registrar-se',
    name: 'Registrarse',
    component: Registrarse,
  },
  {
    path: '/cadastrar-consultas',
    name: 'cadastrar-consultas',
    component: CadastrarConsulta,
    meta: { requiresAuth: true, allowedRoles: ['Administrador'] },
  },
  {
    path: '/disponibilizar-horarios',
    name: 'disponibilizar-horarios',
    component: DisponibilizarHorarios,
    meta: { requiresAuth: true, allowedRoles: ['Administrador'] },
  },
  {
    path: '/pacientes',
    name: 'visualizar-pacientes',
    component: VisualizarPacientesView,
    meta: { requiresAuth: true, allowedRoles: ['Administrador', 'Professor'] },
  },
  {
    path: '/professores',
    name: 'visualizar-professores',
    component: VisualizarProfessoresView,
    meta: { requiresAuth: true, allowedRoles: ['Administrador'] },
  },
  {
    path: '/',
    redirect: '/login',
  },
  {
    path: '/login',
    name: 'Login',
    component: LoginView,
  },
  {
    path: '/cadastroPaciente',
    name: 'CadastroPaciente',
    component: CadastroPaciente,
  },
  {
    path: '/cadastroProfessor',
    name: 'CadastroProfessor',
    component: CadastroProfessor,
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
