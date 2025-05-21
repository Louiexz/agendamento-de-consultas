<template>
  <nav
    class="navbar navbar-expand-lg navbar-dark bg-primary p-3 fixed-top shadow-sm d-flex flex-wrap justify-content-between align-items-center"
  >
    <!-- Logo -->
    <RouterLink to="/">
      <img src="@/assets/Logo.svg" alt="Logo" class="logo" />
    </RouterLink>

    <!-- Botão hamburguer (só aparece em 400px ou menos) -->
    <button
      class="hamburguer-btn"
      type="button"
      @click="isMenuOpen = !isMenuOpen"
    >
      <span class="navbar-toggler-icon"></span>
    </button>

    <!-- Menu do usuário (colapsável no mobile) -->
    <div
      class="user-area d-flex align-items-center gap-3 text-light"
      :class="{ 'd-none-mobile': !isMenuOpen }"
    >
      <div class="text-end">
        <strong>{{ auth.getNomeUsuario() }}</strong> <br />
        <small>{{ auth.getTipoUsuario() }}</small>
      </div>
      <RouterLink
        :to="perfilRoute"
        @click="carregarPerfilUsuarioLogado"
        class="icon"
      >
        <i class="bi bi-person-circle"></i>
      </RouterLink>
      <LogoutButton />
    </div>
  </nav>
</template>

<script>
import { ref, computed } from "vue";
import { useAuthStore } from "@/store/auth";
import { useUsuarioStore } from "@/store/usuario";
import api from "@/services/api";
import LogoutButton from "@/components/Autenticacao/Logout.vue";

export default {
  components: {
    LogoutButton,
  },
  setup() {
    const auth = useAuthStore();
    const usuarioStore = useUsuarioStore();
    const isMenuOpen = ref(false);

    // Computed property para determinar a rota de perfil baseada no tipo de usuário
    const perfilRoute = computed(() => {
      switch (auth.tipoUsuario) {
        case "Administrador":
          return "/perfilAdmin";
        case "Professor":
          return "/perfilProfessor";
        case "Paciente":
          return "/perfilPaciente";
        default:
          return "/";
      }
    });

    // Método para carregar os dados do usuário logado
    const carregarPerfilUsuarioLogado = async () => {
      try {
        // Limpa o perfil atual antes de carregar o novo
        usuarioStore.limparPerfil();
        usuarioStore.setUsuario({});

        // Define o perfil como o do usuário logado e busca os dados
        if (auth.tipoUsuario === "Paciente") {
          usuarioStore.setPerfil("paciente");
          const response = await api.get(`/api/Paciente/${auth.id_Usuario}`);
          usuarioStore.setUsuario(response.data.data);
        } else if (auth.tipoUsuario === "Professor") {
          usuarioStore.setPerfil("professor");
          const response = await api.get(`/api/Professor/${auth.id_Usuario}`);
          usuarioStore.setUsuario(response.data.data);
        } else if (auth.tipoUsuario === "Administrador") {
          usuarioStore.setPerfil("admin");
          const response = await api.get(`/api/Admin/${auth.id_Usuario}`);
          usuarioStore.setUsuario(response.data.data);
        }
      } catch (error) {
        console.error("Erro ao carregar perfil do usuário logado:", error);
      }
    };

    return {
      auth,
      isMenuOpen,
      perfilRoute,
      carregarPerfilUsuarioLogado,
    };
  },
};
</script>

<style>
.logo {
  width: 120px;
}
.icon {
  text-decoration: none;
}
.bi-person-circle {
  font-size: 40px;
  color: white;
  cursor: pointer;
  transition: 0.3s ease;
}
.bi-person-circle:hover {
  color: #d8bd2c;
}

.bg-primary {
  background-color: #186fc0 !important;
}

/* Transição do botão hamburguer */
.hamburguer-btn {
  display: none;
  background: none;
  border: none;
  opacity: 0; /* Inicialmente invisível */
  transition: opacity 0.3s ease-in-out; /* Adiciona uma transição suave para o botão */
}

/* Exibe o botão apenas até 400px e aplica a transição de visibilidade */
@media (max-width: 400px) {
  .hamburguer-btn {
    display: block;
    opacity: 1; /* Botão visível quando a tela for pequena */
  }

  .navbar {
    gap: 0.5rem;
  }

  .user-area {
    width: 100%;
    height: 10vh;
    justify-content: end !important;
  }

  .bi-person-circle {
    display: none;
  }

  .d-none-mobile {
    display: none !important;
  }
}
</style>
