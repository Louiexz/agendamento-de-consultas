<template>
  <div id="app">
    <!--<BtnVoltar />-->
    <Registrese />
    <!-- Componente Login renderizado diretamente aqui -->
  </div>
</template>

<script>
import Registrese from "@/components/Registrarse.vue"; // Importe o componente Login
//import BtnVoltar from "@/components/btnVoltar.vue"; // Importe o componente Login

export default {
  components: {
    Registrese, // Registre o componente com a inicial maiúscula
   // BtnVoltar,
  },
};
</script>
