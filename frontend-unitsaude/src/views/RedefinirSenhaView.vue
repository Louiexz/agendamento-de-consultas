<template>
  <div id="app">
    <RedefinirSenha />  <!-- Componente Login renderizado diretamente aqui -->
  </div>
</template>

<script>
import RedefinirSenha from "@/components/RedefinirSenha.vue";  // Importe o componente Login


export default {
  components: {
    RedefinirSenha,
  },
};
</script>
