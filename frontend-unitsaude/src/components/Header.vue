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
        <strong>{{ auth.nomeUsuario }}</strong> <br />
        <small>{{ auth.tipoUsuario }}</small>
      </div>
      <i class="bi bi-person-circle"></i>
      <LogoutButton />
    </div>
  </nav>
</template>

<script>
import { ref } from "vue";
import { useAuthStore } from "@/store/auth";
import LogoutButton from "@/components/Logout.vue";

export default {
  components: {
    LogoutButton,
  },
  setup() {
    const auth = useAuthStore();
    const isMenuOpen = ref(false);

    return {
      auth,
      isMenuOpen,
    };
  },
};
</script>

<style>
.logo {
  width: 120px;
}

.bi-person-circle {
  font-size: 40px;
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
