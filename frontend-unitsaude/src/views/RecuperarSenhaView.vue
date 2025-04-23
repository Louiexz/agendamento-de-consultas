<template>
  <div id="app">
    <RecuperarSenha />  <!-- Componente Login renderizado diretamente aqui -->
  </div>
</template>

<script>
import RecuperarSenha from "@/components/RecuperarSenha.vue";  // Importe o componente Login

export default {
  components: {
    RecuperarSenha,  // Registre o componente com a inicial maiúscula
  },
};
</script>
