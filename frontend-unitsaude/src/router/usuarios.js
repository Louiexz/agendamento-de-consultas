// Todos os usuários
import LoginView from '@/views/Autenticacao/LoginView.vue';
import Registrarse from '@/views/Cadastro/RegistrarseView.vue';
import RecuperarSenha from '@/views/Autenticacao/RecuperarSenhaView.vue';
import RedefinirSenha from '@/views/Autenticacao/RedefinirSenhaView.vue';

export default [
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
    path: '/registrar',
    name: 'Registrarse',
    component: Registrarse,
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
  }
]