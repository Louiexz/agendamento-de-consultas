<template>
  <nav class="navbar navbar-expand-lg navbar-dark bg-primary p-3 d-flex justify-content-between align-items-center fixed-top shadow-sm">
    <!-- Logo -->
    <RouterLink to="/">
      <img src="@/assets/Logo.svg" alt="Logo" class="logo" />
    </RouterLink>

    <!-- Área do usuário -->
    <div class="d-flex align-items-center gap-3 text-light">
      <div class="text-end">
        <strong>{{ auth.nomeUsuario }}</strong> <br />
        <small>{{ auth.tipoUsuario }}</small>
      </div>
      <i class="bi bi-person-circle"></i>

      <!-- Botão de logout -->
      <LogoutButton />
    </div>
  </nav>
</template>

<script>
import { useAuthStore } from "@/store/auth";
import LogoutButton from "@/components/Logout.vue"; // Ajuste o caminho se necessário!

export default {
  components: {
    LogoutButton,
  },
  setup() {
    const auth = useAuthStore();

    return {
      auth,
    };
  },
};
</script>

<style scoped>
.logo {
  width: 120px;
}

.bi-person-circle 
{
  font-size: 40px;
}

.bg-primary {
  background-color: #186fc0 !important; /* seu azul padrão */
}
</style>
