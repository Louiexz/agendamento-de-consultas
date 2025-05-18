import { createRouter, createWebHistory } from "vue-router";

//Todos os usuários
import usual_routes from "./usuarios.js";

//Admin
import admin_routes from "./admin.js";

//Paciente
import paciente_routes from "./paciente.js";

//Professor
import professor_routes from "./professor.js";

//Admin e Paciente
import CadastrarConsulta from "@/views/Cadastro/CadastrarConsultaView.vue";

// Admin e Professor
import VisualizarPacientesView from "@/views/Visualizar/VisualizarPacientesView.vue";
import PerfilProfessor from "@/views/Visualizar/PerfilProfessorView.vue";
import PerfilAdmin from "@/views/Visualizar/PerfilAdminView.vue";

// Usuários logados
import PerfilPaciente from "@/views/Visualizar/PerfilPacienteView.vue";

import { useAuthStore } from "@/store/auth";
import { useUsuarioStore } from "@/store/usuario";

const getAuth = () => useAuthStore();
const getPerfil = () => useUsuarioStore();

const routes = [
  ...usual_routes,
  ...admin_routes,
  ...paciente_routes,
  ...professor_routes,
  {
    path: "/cadastrar-consultas",
    name: "cadastrar-consultas",
    component: CadastrarConsulta,
    meta: { requiresAuth: true, allowedRoles: ["Administrador", "Paciente"] },
  },
  {
    path: "/pacientes",
    name: "visualizar-pacientes",
    component: VisualizarPacientesView,
    meta: { requiresAuth: true, allowedRoles: ["Administrador", "Professor"] },
  },
  {
    path: "/perfilPaciente",
    name: "perfil-paciente",
    component: PerfilPaciente,
    meta: {
      requiresAuth: true,
      allowedRoles: ["Administrador", "Professor", "Paciente"],
    },
  },
  {
    path: "/perfilProfessor",
    name: "perfil-professor",
    component: PerfilProfessor,
    meta: { requiresAuth: true, allowedRoles: ["Administrador", "Professor"] },
  },

  {
    path: "/perfilAdmin",
    name: "perfil-administrador",
    component: PerfilAdmin,
    meta: { requiresAuth: true, allowedRoles:"Administrador" },
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

router.beforeEach((to, from, next) => {
  const auth = getAuth();
  const perfil = getPerfil();

  if (auth.token && to.path === "/login") {
    if (auth.tipoUsuario === "Administrador") {
      return next("/admin");
    } else if (auth.tipoUsuario === "Professor") {
      return next("/professor");
    } else if (auth.tipoUsuario === "Paciente") {
      return next("/paciente");
    }
  }

  if (to.meta.requiresAuth) {
    if (!auth.token) {
      return next("/login");
    }

    if (
      to.meta.allowedRoles &&
      !to.meta.allowedRoles.includes(auth.tipoUsuario)
    ) {
      return next("/login");
    }
  }

  // Atualiza o perfil na store com base na navegação de página
  if (
    from.name === "visualizar-pacientes" &&
    perfil.perfilAtual !== "paciente"
  ) {
    perfil.setPerfil("paciente"); // Se vem de 'visualizar-pacientes', define o perfil como 'paciente'
  }

  if (
    from.name === "visualizar-professores" &&
    perfil.perfilAtual !== "professor"
  ) {
    perfil.setPerfil("professor"); // Se vem de 'visualizar-professores', define o perfil como 'professor'
  }

  // Logica para navegação entre as páginas de perfil
  if (to.name === "perfil-paciente") {
    if (perfil.perfilAtual !== "paciente") {
      return next("/pacientes"); // Bloqueia a navegação se o perfil não for 'paciente'
    }
  }

  if (to.name === "perfil-professor") {
    if (perfil.perfilAtual !== "professor") {
      return next("/professores"); // Bloqueia a navegação se o perfil não for 'professor'
    }
  }

  next();
});

export default router;
