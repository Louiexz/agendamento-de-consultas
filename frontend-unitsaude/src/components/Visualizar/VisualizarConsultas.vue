<template>
  <div class="container shadow p-4">
    <Header />
    <div class="title mb-4">
      <BackButton />
      <h3>Agendamentos {{ area }}</h3>    
    </div>
    <Filtro :area="area" :isPaciente="localPaciente"/>
  </div>
</template>

<script>
import api from "@/services/api";
import BackButton from "@/components/btnVoltar.vue";
import Header from "@/components/Header.vue";
import Filtro from "@/components/FiltrarConsulta.vue"
import { useAuthStore } from "@/store/auth";

export default {
  data() {
    return {
      auth: null,
      localPaciente: {
        status: false,
        name: "",
        id: -1,
      }
    }
  },
  props: {
    area: {
      type: String,
      required: true,
    },
    isPaciente: {
      type: Boolean,
      required: false,
      default: false,
    },
  },
  mounted() {
    this.auth = useAuthStore();
    
    if (this.isPaciente){
      this.localPaciente = {
        status: true, 
        name: this.auth.nomeUsuario, 
        id: this.auth.id_Usuario
      }
    }
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
  margin-top: 7rem;
  margin-bottom: 2rem;
}

.title {
  display: flex;
  align-items: center;
  gap: 1rem;
}
</style>