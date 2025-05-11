<template>
  <div class="tela-paciente">
    <Header />
    <PacienteHome />
  </div>
</template>

<script>
import Header from '@/components/Header.vue'
import PacienteHome from '@/components/PacienteHome.vue'

export default {
  name: 'TelaPaciente',
  components: {
    Header,
    PacienteHome
  }
}
</script>

<style scoped>
.tela-paciente {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}
</style>