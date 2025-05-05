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
        <h4 class="title">Consultas em espera</h4>

        <div
          v-if="temConsultas('Em Espera')"
          v-for="consulta in consultasFiltradas('Em Espera')"
          :key="consulta.id_Consultas"
          class="consulta">
          <ConsultaView :consulta="consulta"/>
        </div>

        <span
          v-else
          class="aviso"
          >Sem consultas no momento.</span>
      </div>
      <div id="consultas-agendadas">
        <h4 class="title">Consultas agendadas</h4>
        <span v-if="erro" class="alert alert-danger d-block">{{ erro }}</span>

        <div
          v-if="temConsultas('Agendada')"
          v-for="consulta in consultasFiltradas('Agendada')"
          :key="consulta.id_Consultas">
          <ConsultaView :consulta="consulta"/>
        </div>

        <span
          v-else
          class="aviso"
          >Sem consultas, no momento.</span>
      </div>
    </div>
  </div>
</template>

<script>
import api from "@/services/api";
import BackButton from "@/components/btnVoltar.vue";
import Header from "@/components/Header.vue";
import ConsultaView from "@/components/Consulta";

export default {
  props: {
    area: {
      type: String,
      required: true
    }
  },
  components: {
    Header,
    BackButton,
    ConsultaView
  },
  data() {
    return {
      erro: "",
      consultas: [],
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
.aviso {
  text-align: center;
  padding: 10px
}
@media screen and (max-width: 700px) {
  .consultas {
    padding: 1rem 5rem;
    gap: 2rem;
    flex-direction: column;
  }
}
</style>