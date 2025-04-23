<template>
  <h2>Tela de Administrador</h2>
  <!-- O botão de logout só é exibido se o usuário estiver logado -->
  <LogoutButton v-if="isLoggedIn" />
</template>

<script>
import { useAuthStore } from '@/store/auth';
import { computed, onMounted } from 'vue'; // Importando o necessário
import LogoutButton from '@/components/Logout.vue';

export default {
  components: {
    LogoutButton,
  },
  setup() {
    const auth = useAuthStore();

    // Computed para verificar se o usuário está logado
    const isLoggedIn = computed(() => auth.token !== null);

    // Garantindo que o token seja carregado ao montar o componente
    onMounted(() => {
      auth.carregarToken(); // Carrega o token do localStorage
    });

    return {
      isLoggedIn,
    };
  }
}
</script>
