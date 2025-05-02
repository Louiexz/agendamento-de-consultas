<template>
  <div id="app">
    <!--<BtnVoltar />-->
    <CadastroPaciente />
    <!-- Componente Login renderizado diretamente aqui -->
  </div>
</template>

<script>
import CadastroPaciente from "@/components/Cadastro/CadastroPaciente.vue"; // Importe o componente Login
//import BtnVoltar from "@/components/btnVoltar.vue"; // Importe o componente Login

export default {
  components: {
    CadastroPaciente, // Registre o componente com a inicial maiúscula
   // BtnVoltar,
  },
};
</script>
