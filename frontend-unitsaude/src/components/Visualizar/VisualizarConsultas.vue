<template>
  <div>
    <!-- Componente Header -->
    <Header />
    <div class="main-title">
      <BackButton class="voltar"/>
      <h3>Agendamentos {{area}}</h3>
    </div>
    <div class="main d-flex justify-content-center consultas">
      <div id="consultas-em-espera">
        <div
          class="title">
          <h4>Consultas em espera</h4>
        </div>

        <div
          v-if="temConsultas('Em Espera')"
          v-for="consulta in consultasFiltradas('Em Espera')"
          :key="consulta.id_Consultas"
          class="consulta">
          <span><b>{{ consulta.data }} às {{ consulta.horario }}</b></span>
          <span><i class="bi bi-clipboard2-pulse-fill"></i> {{ consulta.area }}</span>
          <span><i class="bi bi-geo-alt-fill"></i> Centro Universitário Tiradentes - UNIT PE</span>
          <RouterLink :to="`/consulta/${consulta.id_Consulta}`" class="consulta-detalhes">Ver detalhes</RouterLink>
        </div>

        <span
          v-else
          class="aviso"
          >Sem consultas, por favor, volte em outro momento.</span>
      </div>
      <div id="consultas-agendadas">
        <div class="title">
          <h4>Consultas agendadas</h4>
        </div>
        <span v-if="erro" class="alert alert-danger d-block">{{ erro }}</span>

        <div
          v-if="temConsultas('Agendada')"
          v-for="consulta in consultasFiltradas('Agendada')"
          :key="consulta.id_Consultas"
          class="consulta">
          <span><b>{{ consulta.data }} às {{ consulta.horario }}</b></span>
          <span><i class="bi bi-clipboard2-pulse-fill"></i> {{ consulta.area }}</span>
          <span><i class="bi bi-geo-alt-fill"></i> Centro Universitário Tiradentes - UNIT PE</span>
          <RouterLink :to="`/consulta/${consulta.id_Consulta}`" class="consulta-detalhes">Ver detalhes</RouterLink>
        </div>

        <span
          v-else
          class="aviso"
          >Sem consultas, por favor, volte em outro momento.</span>
      </div>
    </div>
  </div>
</template>

<script>
import api from "@/services/api";
import BackButton from "@/components/btnVoltar.vue";
import Header from "@/components/Header.vue";

export default {
  props: {
    area: {
      type: String,
      required: true
    }
  },
  components: {
    Header,
    BackButton
  },
  data() {
    return {
      erro: "",
      consultas: []
    };
  },
  methods: {      
    consultasFiltradas(status) {
      if (this.consultas != null){
        return this.consultas.filter(
          (c) => c.status === status && c.area === this.area
        );
      }
      return;
    },
    temConsultas(status) {
      let resultado = this.consultasFiltradas(status);
      if (resultado) {
        return resultado.length > 0;    
      }
      return;
    },
    async getConsultaPorArea() {
      try {
        const response = await api.get("api/Consulta/FiltrarConsultas", {
          params: {
            Area: this.area
          }
        });

        if (response.status && response.data != null) {
          this.consultas = response.data.data;
        } else {
          this.erro = response.message;
        }
      } catch (error) {
        this.erro = error;
        console.error("Erro ao buscar consultas:", erro);
      }
    }
  },
  async mounted() {
    await this.getConsultaPorArea();
  }
};
</script>

<style scoped>
.title {
  background-color: #186fc0;
  padding: 10px;
  border-top-left-radius: 15px;
  border-top-right-radius: 15px;
  color: white;
  text-align: center;
}
.main-title {
  display: flex;
  align-items: center;
  padding-top: 7rem;
  padding-left: 1rem;
  padding-bottom: 2rem;
  gap: 10%;
}
.consultas {
  gap: 12%;
  padding: 0rem 1rem;
}
.consulta-detalhes {
  text-align: center;
  min-width: 100%
}
.aviso {
  text-align: center;
  padding: 10px
}
.consulta {
  display: flex;
  flex-direction: column;
  box-shadow: 1px 1px 1px 1px;
  align-items: flex-start;
  padding: 20px;
  gap: 20px;
  min-width: 90%
}
@media screen and (max-width: 700px) {
  .consultas {
    padding: 1rem 5rem;
    gap: 2rem;
    flex-direction: column;
  }
}
</style>