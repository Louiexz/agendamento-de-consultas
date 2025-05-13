<template>
  <div class="container shadow p-4">
    <Header />
    <div class="title mb-4">
      <BackButton />
      <h3>Agendamentos {{ area }}</h3>    
    </div>
    <Filtro :area="area"/>
  </div>
</template>

<script>
import api from "@/services/api";
import BackButton from "@/components/btnVoltar.vue";
import Header from "@/components/Header.vue";
import Filtro from "@/components/FiltrarConsulta.vue"
export default {
  props: {
    area: {
      type: String,
      required: true,
    },
  },
  components: {
    Header,
    BackButton,
    Filtro
  },
};
</script>

<style scoped>
.container {
  margin-top: 6rem;
  margin-bottom: 2rem;
}

.title {
  display: flex;
  align-items: center;
  gap: 1rem;
}
</style>