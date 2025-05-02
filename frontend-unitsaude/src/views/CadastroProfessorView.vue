<template>
    <div id="app">
      <BtnVoltar />
      <CadastroProfessor />
    </div>
  </template>
  
  <script>
  import CadastroProfessor from "@/components/Cadastro/CadastroProfessor.vue";
  import BtnVoltar from "@/components/btnVoltar.vue";
  
  export default {
    components: {
      CadastroProfessor, // Registre o componente com a inicial maiúscula
      BtnVoltar,
    },
  };
  </script>
  
