// Professor
import TelaProfessor from '@/views/TelaProfessor.vue';

export default [
  {
    path: '/professor',
    name: 'professor',
    component: TelaProfessor,
    meta: { requiresAuth: true, allowedRoles: ['Professor'] },
  },
]