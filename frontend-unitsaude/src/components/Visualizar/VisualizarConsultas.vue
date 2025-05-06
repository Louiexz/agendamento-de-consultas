<template>
  <div class="container shadow p-4">
    <!-- Header -->
    <Header />
    <div class="title mb-4">
      <BackButton />
      <h3>Agendamentos {{ area }}</h3>
    </div>


    <div class="row gap-4 justify-content-center">
      <!-- Consultas em espera -->
      <div class="col-md-5">
        <h4 class="title-2">Consultas em espera</h4>
        <div
          :class="{
            'consulta-list': true,
            scrollable: temConsultas('Em Espera'),
          }"
        >
          <template v-if="temConsultas('Em Espera')">
            <div
              v-for="consulta in consultasFiltradas('Em Espera')"
              :key="consulta.id_Consultas"
              class="consulta"
            >
              <ConsultaView :consulta="consulta" />
            </div>
          </template>
          <span v-else class="aviso">Sem consultas no momento.</span>
        </div>
      </div>

      <!-- Consultas agendadas -->
      <div class="col-md-5">
        <h4 class="title-2">Consultas agendadas</h4>
        <span v-if="erro" class="alert alert-danger d-block">{{ erro }}</span>
        <div
          :class="{
            'consulta-list': true,
            scrollable: temConsultas('Agendada'),
          }"
        >
          <template v-if="temConsultas('Agendada')">
            <div
              v-for="consulta in consultasFiltradas('Agendada')"
              :key="consulta.id_Consultas"
              class="consulta"
            >
              <ConsultaView :consulta="consulta" />
            </div>
          </template>
          <span v-else class="aviso">Sem consultas, no momento.</span>
        </div>
      </div>

          <!-- Spinner durante carregamento -->
    <div v-if="isLoading" class="text-center my-5">
      <div
        class="spinner-border text-primary"
        role="status"
        style="width: 3rem; height: 3rem"
      >
        <span class="visually-hidden">Carregando...</span>
      </div>
      <p class="mt-3">Carregando consultas...</p>
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
      required: true,
    },
  },
  components: {
    Header,
    BackButton,
    ConsultaView,
  },
  data() {
    return {
      erro: "",
      consultas: [],
      isLoading: false, // Adicione esta linha
    };
  },
  methods: {
    consultasFiltradas(status) {
      if (this.consultas != null) {
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
      this.isLoading = true; // Ativa o loading
      this.erro = ""; // Limpa erros anteriores
      try {
        const response = await api.get("api/Consulta/FiltrarConsultas", {
          params: {
            Area: this.area,
          },
        });

        if (response.status && response.data != null) {
          this.consultas = response.data.data;
        } else {
          this.erro = response.message;
        }
      } catch (error) {
        this.erro = error;
        console.error("Erro ao buscar consultas:", erro);
      } finally {
        this.isLoading = false; // Desativa o loading
      }
    },
  },
  async mounted() {
    await this.getConsultaPorArea();
  },
};
</script>

<style scoped>
.container {
  margin-top: 8rem;
}
h3 {
  margin: 0;
}
.title {
  display: grid;
  grid-template-columns: auto 1fr;
  align-items: center;
  text-align: center;
  margin-bottom: 1rem;

  padding: 10px;
  border-radius: 10px;
}
.title-2 {
  align-items: center;
  text-align: center;
  margin-bottom: 1rem;
  background-color: #186fc0;
  color: white;
  padding: 10px;
  border-radius: 10px;
}
.aviso {
  text-align: center;
  padding: 10px;
  color: #666;
}

.consulta-list {
  max-height: 60vh;
  padding-right: 10px;
  margin-top: 1rem;
}
/* Adiciona a rolagem apenas se a classe 'scrollable' for aplicada */
.consulta-list.scrollable {
  overflow-y: auto;
}
.consulta-list::-webkit-scrollbar {
  width: 6px;
}
.consulta-list::-webkit-scrollbar-thumb {
  background-color: #186fc0;
  border-radius: 10px;
}
.consulta-list::-webkit-scrollbar-track {
  background-color: #f1f1f1;
}

@media screen and (max-width: 700px) {
  .consultas {
    padding: 1rem 5rem;
    gap: 2rem;
    flex-direction: column;
  }
}
</style>
