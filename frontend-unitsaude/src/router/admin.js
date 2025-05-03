//Admin
import TelaAdmin from '@/views/TelaAdmin.vue';
import CadastroPaciente from '@/views/Cadastro/CadastroPacienteView.vue';
import CadastroProfessor from '@/views/Cadastro/CadastroProfessorView.vue';
import DisponibilizarHorarios from '@/views/Cadastro/DisponibilizarHorariosView.vue';
import VisualizarProfessoresView from '@/views/Visualizar/VisualizarProfessoresView.vue';

export default [
  {
    path: '/professores',
    name: 'visualizar-professores',
    component: VisualizarProfessoresView,
    meta: { requiresAuth: true, allowedRoles: ['Administrador'] },
  },
  {
    path: '/cadastroProfessor',
    name: 'CadastroProfessor',
    component: CadastroProfessor,
    meta: { requiresAuth: true, allowedRoles: ['Administrador'] },
  },
  {
    path: '/admin',
    name: 'admin',
    component: TelaAdmin,
    meta: { requiresAuth: true, allowedRoles: ['Administrador'] },
  },
  {
    path: '/disponibilizar-horarios',
    name: 'disponibilizar-horarios',
    component: DisponibilizarHorarios,
    meta: { requiresAuth: true, allowedRoles: ['Administrador'] },
  },
  {
    path: '/cadastroPaciente',
    name: 'CadastroPaciente',
    component: CadastroPaciente,
    meta: { requiresAuth: true, allowedRoles: ['Administrador'] },
  },
]