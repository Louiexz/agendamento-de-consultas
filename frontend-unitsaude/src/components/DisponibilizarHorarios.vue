<template>
  <Header />
  <BackButton class="voltar" />
  <div class="main d-flex justify-content-center align-items-center">
    <div class="form card">
      <div class="Title">
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
        <div class="form-group botaocentro">
          <button type="submit" class="btn btn-primary">
            Disponibilizar Horários
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import api from "@/services/api"; // Importando o serviço de API
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
    };
  },
  methods: {
    // Método para obter as áreas
    async obterAreas() {
      try {
        const response = await api.get("/api/Consulta/areas");
        this.areas = response.data.data; // Armazenando as áreas
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
        this.especialidades = response.data.data; // Armazenando as especialidades
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

      try {
        const disponibilidade = {
          dataInicio: this.dataInicio, // ← direto
          dataFim: this.dataFim, // ← direto
          horarioInicio: this.horarioInicio, // ← direto
          horarioFim: this.horarioFim,
          area: this.selectedArea,
          especialidade: this.selectedEspecialidade,
          ativo: true,
        };

        console.log(disponibilidade);

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

        this.selectedArea = "";
        this.selectedEspecialidade = "";
        this.dataInicio = "";
        this.dataFim = "";
        this.horarioInicio = "";
        this.horarioFim = "";
        this.especialidades = [];
      } catch (error) {
        console.error("Erro ao disponibilizar horário:", error);
        Swal.fire({
          icon: "error",
          title: "Erro",
          text: "Ocorreu um erro ao disponibilizar os horários. Tente novamente.",
          background: "#ffffff",
          color: "#186fc0",
          confirmButtonColor: "#d8bd2c",
        });
      }
    },
  },
  mounted() {
    // Carregar as áreas ao montar o componente
    this.obterAreas();
  },
};
</script>

<style scoped>
/* Estilos para o formulário */
.form {
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 500px;
  background-color: #fff;
}

.main {
  margin-top: 20vh;
}

.voltar {
  position: absolute;
  top: 7rem;
  left: 1vw;
  z-index: 100000;
}

.form-group {
  margin-bottom: 1.5rem;
}

.Title {
  text-align: center;
}

.agroup {
  display: grid;
  grid-template-columns: repeat(2, 1fr); /* 2 colunas com largura igual */
  gap: 1rem; /* Espaçamento entre os itens */
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

.botaocentro {
  text-align: center;
}


/* Responsividade geral */
@media (max-width: 768px) {
  .form {
    padding: 1.5rem;
    width: 100%;
    box-sizing: border-box;
  }

  .agroup {
    grid-template-columns: 1fr; /* Uma coluna em telas menores */
  }

.voltar {display: none;}
  .main {
    margin-top: 15vh;
  }
}

@media (max-width: 480px) {
  .main {
    margin-top: 15vh;
  }

  h2 {
    font-size: 1.3rem;
  }
}
</style>
