<template>
  <div id="app">
    <Login />
    <!-- Componente Login renderizado diretamente aqui -->
  </div>
</template>

<script>
import Login from "@/components/Login.vue";  // Importe o componente Login

export default {
  name: "App",
  components: {
    Login,  // Registre o componente com a inicial maiúscula
  }
}
</script>