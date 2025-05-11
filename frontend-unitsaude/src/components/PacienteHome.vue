<template>
  <div class="paciente-home">
    <h1>Bem-vindo, Paciente!</h1>
    <!-- Conteúdo específico do paciente -->
    <router-link to="/consultas">Minhas Consultas</router-link>
  </div>
</template>

<script>
export default {
  name: 'PacienteHome'
}
</script>

<style scoped>
.paciente-home {
  padding: 20px;
  background-color: #e3f2fd;
}
</style>