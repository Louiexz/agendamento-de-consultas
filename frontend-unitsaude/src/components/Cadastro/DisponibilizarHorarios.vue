<template>
  <Header />

  <div class="main-container d-flex justify-content-center align-items-center">
    <!-- Formulário -->
    <div class="form card">
      <div class="Title">
        <BackButton />
        <h2>Disponibilizar Horários</h2>
      </div>
      <!-- Formulário -->
      <form @submit.prevent="enviarDisponibilidade">
        <div class="form-group">
          <label for="area">Área</label>
          <select
            v-model="selectedArea"
            @change="obterEspecialidades"
            id="area"
            class="form-control"
            required
          >
            <option value="" disabled selected>Selecione uma área...</option>
            <option v-for="area in areas" :key="area" :value="area">
              {{ area }}
            </option>
          </select>
        </div>

        <div class="form-group">
          <label for="especialidade">Especialidade</label>
          <select
            v-model="selectedEspecialidade"
            id="especialidade"
            class="form-control"
            required
          >
            <option value="" disabled selected>
              Selecione uma especialidade...
            </option>
            <option
              v-for="especialidade in especialidades"
              :key="especialidade"
              :value="especialidade"
            >
              {{ especialidade }}
            </option>
          </select>
        </div>

        <div class="agroup">
          <div class="form-group">
            <label for="dataInicio">Data de Início</label>
            <input
              type="date"
              v-model="dataInicio"
              id="dataInicio"
              class="form-control"
              required
            />
          </div>

          <div class="form-group">
            <label for="dataFim">Data de Fim</label>
            <input
              type="date"
              v-model="dataFim"
              id="dataFim"
              class="form-control"
              required
            />
          </div>

          <div class="form-group">
            <label for="horarioInicio">Horário de Início</label>
            <input
              type="time"
              v-model="horarioInicio"
              id="horarioInicio"
              class="form-control"
              required
            />
          </div>

          <div class="form-group">
            <label for="horarioFim">Horário de Fim</label>
            <input
              type="time"
              v-model="horarioFim"
              id="horarioFim"
              class="form-control"
              required
            />
          </div>
        </div>
        <div class="botaocentro">
          <button type="submit" class="btn btn-primary" :disabled="isLoading">
            <span v-if="!isLoading">Disponibilizar Horários</span>
            <span
              v-else
              class="spinner-border spinner-border-sm"
              role="status"
              aria-hidden="true"
            ></span>
          </button>
        </div>
      </form>
    </div>

    <!-- Lista de disponibilidades existentes -->
    <div class="disponibilidades-list card">
      <h3>Disponibilidades Cadastradas</h3>

      <!-- Loading -->
      <div v-if="isLoadingDisponibilidades" class="text-center my-3">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Carregando...</span>
        </div>
        <p>Carregando disponibilidades...</p>
      </div>

      <!-- Mensagem quando não há disponibilidades -->
      <div v-else-if="disponibilidades.length === 0" class="alert alert-info">
        Nenhuma disponibilidade cadastrada.
      </div>

      <!-- Lista de disponibilidades -->
      <div class="disponibilidades-container">
        <div
          v-for="disponibilidade in disponibilidades"
          :key="disponibilidade.id"
          class="disponibilidade-item card p-2 mb-2 shadow-sm"
        >
          <div class="d-flex justify-content-between">
            <strong
              >{{ disponibilidade.area }} -
              {{ disponibilidade.especialidade }}</strong
            >
            <span
              class="status"
              :class="{
                ativo: disponibilidade.ativo,
                inativo: !disponibilidade.ativo,
              }"
            >
              {{ disponibilidade.ativo ? "Ativo" : "Inativo" }}
            </span>
          </div>
          <div>
            De {{ formatDate(disponibilidade.dataInicio) }} até
            {{ formatDate(disponibilidade.dataFim) }}
          </div>
          <div>
            Horário: {{ disponibilidade.horarioInicio }} às
            {{ disponibilidade.horarioFim }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import api from "@/services/api";
import Header from "@/components/Header.vue";
import BackButton from "@/components/btnVoltar.vue";
import Swal from "sweetalert2";

export default {
  components: {
    Header,
    BackButton,
  },
  data() {
    return {
      areas: [],
      especialidades: [],
      selectedArea: "",
      selectedEspecialidade: "",
      dataInicio: "",
      dataFim: "",
      horarioInicio: "",
      horarioFim: "",
      erro: null,
      isLoading: false,
      disponibilidades: [],
      isLoadingDisponibilidades: false,
    };
  },
  methods: {
    // Método para formatar data
    formatDate(dateString) {
      if (!dateString) return "";
      const date = new Date(dateString);
            date.setHours(date.getHours() + 3);
      return date.toLocaleDateString("pt-BR");
    },

    // Método para obter as disponibilidades existentes
    async obterDisponibilidades() {
      this.isLoadingDisponibilidades = true;
      try {
        const response = await api.get(
          "/api/Disponibilidade/GetTodasDisponibilidades"
        );
        // Adiciona o campo ativo=true para todas as disponibilidades
        // (como não vem do backend, assumimos que todas estão ativas)
        this.disponibilidades = (response.data.data || []).map((item) => ({
          ...item,
          ativo: true, // Valor padrão, pois o DTO não retorna essa informação
        }));
      } catch (error) {
        console.error("Erro ao carregar disponibilidades:", error);
        this.erro =
          "Erro ao carregar disponibilidades. Tente novamente mais tarde.";
      } finally {
        this.isLoadingDisponibilidades = false;
      }
    },

    // Método para obter as áreas
    async obterAreas() {
      try {
        const response = await api.get("/api/Consulta/areas");
        this.areas = response.data.data;
        console.log("Áreas carregadas:", this.areas);
      } catch (error) {
        console.error("Erro ao carregar áreas:", error);
        this.erro = "Erro ao carregar áreas. Tente novamente mais tarde.";
      }
    },

    // Método para obter as especialidades com base na área selecionada
    async obterEspecialidades() {
      console.log("Área selecionada:", this.selectedArea);
      try {
        const response = await api.get(
          `/api/Consulta/especialidades/${this.selectedArea}`
        );
        this.especialidades = response.data.data;
      } catch (error) {
        console.error("Erro ao carregar especialidades:", error);
        this.erro =
          "Erro ao carregar especialidades. Tente novamente mais tarde.";
      }
    },

    // Método para enviar os dados de disponibilidade de horários
    async enviarDisponibilidade() {
      console.log("Especialidade selecionada:", this.selectedEspecialidade);
      if (
        !this.selectedArea ||
        !this.selectedEspecialidade ||
        !this.dataInicio ||
        !this.dataFim ||
        !this.horarioInicio ||
        !this.horarioFim
      ) {
        Swal.fire({
          icon: "error",
          title: "Erro",
          text: "Por favor, preencha todos os campos antes de enviar.",
          background: "#ffffff",
          color: "#186fc0",
          confirmButtonColor: "#d8bd2c",
        });
        return;
      }

      this.isLoading = true;
      this.erro = null;

      try {
        const disponibilidade = {
          dataInicio: this.dataInicio,
          dataFim: this.dataFim,
          horarioInicio: this.horarioInicio,
          horarioFim: this.horarioFim,
          area: this.selectedArea,
          especialidade: this.selectedEspecialidade,
          ativo: true,
        };

        const response = await api.post(
          "/api/Disponibilidade/CreateDisponibilidade",
          disponibilidade
        );

        await Swal.fire({
          icon: "success",
          title: "Horários disponibilizados com sucesso!",
          background: "#ffffff",
          color: "#186fc0",
          confirmButtonColor: "#d8bd2c",
          timer: 2000,
          timerProgressBar: true,
          showConfirmButton: false,
        });

        // Limpa os campos
        this.selectedArea = "";
        this.selectedEspecialidade = "";
        this.dataInicio = "";
        this.dataFim = "";
        this.horarioInicio = "";
        this.horarioFim = "";
        this.especialidades = [];

        // Atualiza a lista de disponibilidades
        await this.obterDisponibilidades();
      } catch (error) {
        console.error("Erro ao disponibilizar horário:", error);
        let errorMessage =
          "Ocorreu um erro ao disponibilizar os horários. Tente novamente.";

        if (
          error.response &&
          error.response.data &&
          error.response.data.message
        ) {
          errorMessage = error.response.data.message;
        }
        Swal.fire({
          icon: "error",
          title: "Erro",
          text: errorMessage,
          background: "#ffffff",
          color: "#186fc0",
          confirmButtonColor: "#d8bd2c",
        });
      } finally {
        this.isLoading = false;
      }
    },
  },
  async mounted() {
    // Carregar as áreas e disponibilidades ao montar o componente
    await this.obterAreas();
    await this.obterDisponibilidades();
  },
};
</script>

<style scoped>
.main-container {
  margin-top: 20vh;
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  padding: 0 2rem;
  align-items: start;
}

.form {
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  background-color: #fff;
  height: fit-content;
}

.disponibilidades-list {
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  background-color: #fff;
  max-height: 70vh;
  display: flex;
  flex-direction: column;
}

.disponibilidades-container {
  overflow-y: auto;
  max-height: 60vh;
  padding-right: 10px;
}

.disponibilidades-container::-webkit-scrollbar {
  width: 5px;
}

.disponibilidades-container::-webkit-scrollbar-thumb {
  background-color: #186fc0;
  border-radius: 10px;
}

.disponibilidade-item {
  transition: transform 0.3s ease;
  cursor: pointer;
}

.status {
  font-size: 0.8rem;
  padding: 0.2rem 0.5rem;
  border-radius: 4px;
  display: inline-block;
  margin-top: 0.5rem;
}

.ativo {
  background-color: #d4edda;
  color: #155724;
}

.inativo {
  background-color: #f8d7da;
  color: #721c24;
}

.form-group {
  margin-bottom: 1.5rem;
}

.agroup {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
}

select.form-control {
  appearance: none;
  background-color: #f9f9f9;
  border: 1px solid #ced4da;
  padding: 0.5rem 1rem;
  border-radius: 8px;
  font-size: 1rem;
}

select.form-control:hover {
  background-color: #f1f1f1;
}

.btn-primary {
  background-color: #d8bd2c;
  border: none;
  transition: 0.3s ease;
}

.btn-primary:hover {
  background-color: #186fc0;
}

.Title {
  text-align: center;
  display: grid;
  grid-template-columns: auto 1fr;
  align-items: center;
  margin-bottom: 1rem;
  gap: 1rem;
}

.botaocentro {
  text-align: center;
}

/* Responsividade geral */
@media (max-width: 1200px) {
  .main-container {
    grid-template-columns: 1fr;
    margin-top: 15vh;
  }

  .disponibilidades-list {
    max-height: none;
    margin-top: 2rem;
  }
}

@media (max-width: 768px) {
  .main-container {
    padding: 0 1rem;
    margin-top: 15vh;
  }

  .form,
  .disponibilidades-list {
    padding: 1.5rem;
  }

  .agroup {
    grid-template-columns: 1fr;
  }

  .voltar {
    display: none;
  }
}

@media (max-width: 480px) {
  .main-container {
    margin-top: 15vh;
  }

  h2,
  h3 {
    font-size: 1.3rem;
  }
}
</style>
