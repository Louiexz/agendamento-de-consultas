<template>
  <Header />
  <BackButton class="voltar" />
  <div class="main d-flex justify-content-center align-items-center">
    <div class="form card">
      <div class="Title">
        <h2>Agendar Consulta</h2>
      </div>
      <!-- Formulário -->
      <form @submit.prevent="criarConsulta">
        <div class="consulta d-flex justify-content-center">
          <div class="align-content-center">
            <div class="form-group">
              <label for="area">Área</label>
              <select
                v-model="selectedArea"
                id="area"
                class="form-control"
                required
              >
                <option value="" disabled selected>
                  Selecione uma área...
                </option>
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

            <div class="form-group position-relative">
              <label for="paciente">Paciente</label>
              <input
                v-model="searchPaciente"
                type="text"
                id="paciente"
                class="form-control"
                placeholder="Digite nome ou CPF do paciente"
                autocomplete="off"
              />
              <ul
                v-if="pacientes.length"
                class="list-group position-absolute w-100"
                style="z-index: 1000"
              >
                <li
                  v-for="paciente in pacientes"
                  :key="paciente.id"
                  @click="selecionarPaciente(paciente)"
                  class="list-group-item list-group-item-action"
                  style="cursor: pointer"
                >
                  {{ paciente.nome }} - {{ paciente.cpf }}
                </li>
              </ul>
            </div>

            <div class="form-group">
              <label for="professor">Professor</label>
              <select
                v-model="selectedProfessor"
                id="professor"
                class="form-control"
                required
              >
                <option value="" disabled selected>
                  Selecione um professor...
                </option>
                <option
                  v-for="professor in professores"
                  :key="professor.id"
                  :value="professor.id"
                >
                  {{ professor.nome }}
                </option>
              </select>
            </div>

            <div class="form-group">
              <label for="horario">Horário</label>
              <select
                v-model="horarioConsulta"
                id="horario"
                class="form-control"
                style="height: 45px"
                required
              >
                <option disabled value="">Selecione um horário</option>

                <!-- Grupo de Horários Disponíveis -->
                <optgroup label="Disponíveis">
                  <option
                    v-for="item in horariosDisponiveis.filter(
                      (h) => h.status === 'Disponível'
                    )"
                    :key="item.horario"
                    :value="item.horario"
                  >
                    {{ item.horario }}
                  </option>
                </optgroup>

                <!-- Grupo de Horários Indisponíveis -->
                <optgroup label="Fila de Espera">
                  <option
                    v-for="item in horariosDisponiveis.filter(
                      (h) => h.status !== 'Disponível'
                    )"
                    :key="item.horario + '-fila'"
                    :value="item.horario"
                  >
                    {{ item.horario }}
                  </option>
                </optgroup>
              </select>
            </div>
          </div>

          <div class="form-group calendario-wrapper">
            <!-- <label for="data">Data</label> -->

            <vue-cal
              date-picker
              default-view="month"
              :time="false"
              :locale="'pt-br'"
              :min-date="new Date().toISOString().split('T')[0]"
              :disabled-dates="disableSundays"
              @cell-click="selecionarData"
            />
          </div>
        </div>
        <div class="form-group botaocentro">
          <button type="submit" class="btn btn-primary">Agendar</button>
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
import debounce from "lodash.debounce";
import { VueCal } from "vue-cal";
import "vue-cal/style";

export default {
  components: {
    Header,
    BackButton,
    VueCal,
  },
  data() {
    return {
      areas: [],
      especialidades: [],
      pacientes: [],
      professores: [],
      horariosDisponiveis: [],
      selectedArea: "",
      selectedEspecialidade: "",
      selectedPacienteId: "",
      selectedProfessor: "",
      searchPaciente: "",
      dataConsulta: "",
      horarioConsulta: "",
      statusConsulta: "Agendada",
      erro: null,
    };
  },
  methods: {
    async obterAreas() {
      try {
        const response = await api.get("/api/Consulta/areas");
        this.areas = response.data.data;
      } catch (error) {
        console.error("Erro ao carregar áreas:", error);
        this.erro = "Erro ao carregar áreas. Tente novamente mais tarde.";
      }
    },
    async obterEspecialidades() {
      if (!this.selectedArea) return; // Não tenta carregar especialidades se nenhuma área for selecionada
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
    async obterPacientes() {
      if (this.searchPaciente.length < 2) {
        this.pacientes = [];
        return;
      }
      try {
        const response = await api.get(
          `/api/Paciente/ListarComFiltro?filtro=${this.searchPaciente}`
        );
        this.pacientes = response.data.data.filter((paciente) => {
          const nomeCpf = `${paciente.nome} ${paciente.cpf}`;
          return nomeCpf
            .toLowerCase()
            .includes(this.searchPaciente.toLowerCase());
        });
      } catch (error) {
        console.error("Erro ao carregar pacientes:", error);
        this.erro = "Erro ao carregar pacientes. Tente novamente mais tarde.";
      }
    },
    selecionarPaciente(paciente) {
      this.searchPaciente = `${paciente.nome} - ${paciente.cpf}`;
      this.selectedPacienteId = paciente.id;
      this.pacientes = []; // esconde a lista
    },
    disableSundays(date) {
      const day = new Date(date).getDay(); // Obtemos o dia da semana (0 = Domingo, 1 = Segunda, ...)
      return day === 0; // Retorna true se for domingo
    },
    selecionarData(payload) {
      const dataSelecionada = payload?.cell?.start;
      if (!(dataSelecionada instanceof Date)) {
        console.warn("Formato de célula inesperado:", payload);
        return;
      }

      const day = new Date(dataSelecionada).getDay();
      if (day === 0) {
        Swal.fire({
          icon: "error",
          title: "Domingo não disponível",
          text: "Não é possível agendar uma consulta para domingos.",
          confirmButtonColor: "#d8bd2c",
        });

        return;
      }

      this.dataConsulta = dataSelecionada.toISOString().split("T")[0];
      console.log("Data selecionada:", this.dataConsulta);
    },
    async obterProfessores() {
      try {
        const response = await api.get(
          `/api/Professor/listar-professores-especialidade?especialidade=${this.selectedEspecialidade}`
        );
        console.log(response);
        this.professores = response.data.data;
      } catch (error) {
        console.error("Erro ao carregar professores:", error);
        this.erro = "Erro ao carregar professores. Tente novamente mais tarde.";
      }
    },

    async obterHorariosDisponiveis() {
      if (
        !this.dataConsulta ||
        !this.selectedArea ||
        !this.selectedEspecialidade
      ) {
        return; // Não faz a requisição se algum dado necessário estiver ausente
      }

      try {
        const response = await api.get("/api/Consulta/horarios-disponiveis", {
          params: {
            data: this.dataConsulta, // Certifique-se de que está formatando a data corretamente
            area: this.selectedArea,
            especialidade: this.selectedEspecialidade,
          },
        });

        if (response.data.status) {
          this.horariosDisponiveis = response.data.data;
        } else {
          this.horariosDisponiveis = [];
          Swal.fire({
            icon: "info",
            title: "Nenhum horário disponível",
            text: response.data.message,
            background: "#ffffff",
            color: "#186fc0",
            confirmButtonColor: "#d8bd2c",
          });
        }
      } catch (error) {
        console.error("Erro ao carregar horários disponíveis:", error);
        this.erro = "Erro ao carregar horários. Tente novamente mais tarde.";
      }
    },

    async criarConsulta() {
      if (
        !this.selectedArea ||
        !this.selectedEspecialidade ||
        !this.selectedPacienteId ||
        !this.selectedProfessor ||
        !this.dataConsulta ||
        !this.horarioConsulta
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

      const consulta = {
        data: this.dataConsulta,
        horario: this.horarioConsulta,
        status: this.statusConsulta,
        area: this.selectedArea,
        especialidade: this.selectedEspecialidade,
        pacienteId: this.selectedPacienteId,
        professorId: this.selectedProfessor,
      };

      try {
        const response = await api.post(
          "/api/Consulta/CreateConsulta",
          consulta
        );
        await Swal.fire({
          icon: "success",
          title: "Consulta agendada com sucesso!",
          background: "#ffffff",
          color: "#186fc0",
          confirmButtonColor: "#d8bd2c",
          timer: 2000,
          timerProgressBar: true,
          showConfirmButton: false,
        });

        this.resetForm();
      } catch (error) {
        const mensagemErro =
          error.response && error.response.data && error.response.data.message
            ? error.response.data.message
            : "Erro ao criar consulta. Tente novamente mais tarde.";

        Swal.fire({
          icon: "error",
          title: "Erro",
          text: mensagemErro,
          background: "#ffffff",
          color: "#186fc0",
          confirmButtonColor: "#d8bd2c",
        });
      }
    },
    resetForm() {
      this.selectedArea = "";
      this.selectedEspecialidade = "";
      this.selectedPacienteId = "";
      this.selectedProfessor = "";
      this.dataConsulta = "";
      this.horarioConsulta = "";
      this.statusConsulta = "Agendada";
      this.searchPaciente = ""; // Limpar o campo de busca do paciente
    },
  },
  watch: {
    selectedArea(newArea) {
      // Chama obterEspecialidades sempre que a área for alterada
      this.obterEspecialidades();
      this.obterHorariosDisponiveis(); // Chama quando a área é alterada
    },
    selectedEspecialidade(newEspecialidade) {
      // Chama obterProfessores sempre que a especialidade for alterada
      this.obterProfessores();
      this.obterHorariosDisponiveis(); // Chama quando a especialidade é alterada
    },
    dataConsulta(newData) {
      this.obterHorariosDisponiveis(); // Chama quando a data é alterada
    },
    searchPaciente: debounce(function () {
      this.obterPacientes();
    }, 500), // Espera 500ms após a digitação para disparar a requisição
  },
  mounted() {
    this.obterAreas();
    this.obterPacientes();
  },
};
</script>

<style scoped>
/* Estilos para o formulário */
.form {
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  width: 50%;
  gap: 1rem;
}
.consulta {
  gap: 3rem;
}
.main {
  margin-top: 15vh;
}

.voltar {
  position: relative;
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

.Title h2 {
  text-align: center;
  font-weight: 400;
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

/* Estiliza o contêiner principal do calendário */
:deep(.vuecal) {
  border-radius: 2px !important;
  overflow: hidden;
  background-color: #fff;
}

/* Estiliza o cabeçalho (barra de navegação do mês/ano) */
:deep(.vuecal__title-bar) {
  border-radius: 8px 8px 0 0 !important;
  background-color: #186fc0;
  color: #ffffff;
  padding: 0.75rem 1rem;
  font-size: 1rem;
  font-weight: bold;
}

/* Estilo de hover para as células */
:deep(.vuecal__cell:hover) {
  background-color: #f0f8ff;
  cursor: pointer;
}

/* Impede o hover nas células dos domingos */
:deep(.vuecal__cell--sun:hover) {
  background-color: transparent !important; /* Não altera a cor de fundo nas células de domingo */
  cursor: not-allowed !important; /* Muda o cursor para 'não permitido' nas células de domingo */
}

/* Estilo visual para desabilitar os domingos */
:deep(.vuecal__cell--sun) {
  pointer-events: none; /* Desativa qualquer evento de clique */
  background-color: #f1f1f100; /* Muda a cor de fundo para indicar que está desabilitado */
  color: #cdc3c3; /* Altera a cor do texto para dar um visual "desabilitado" */
  cursor: not-allowed; /* Muda o cursor para indicar que não é interativo */
}
:deep(.vuecal__cell--disabled:hover) {
  pointer-events: none; /* Desativa qualquer evento de clique */
  cursor: not-allowed; /* Muda o cursor para indicar que não é interativo */
}

:deep(.vuecal__cell-date) {
  font-size: 0.9rem !important; /* Aumenta o tamanho do número do dia */
  text-align: center;
}

/* Célula selecionada */
:deep(.vuecal__cell--selected) {
  background-color: #d8bd2c !important;
  color: white !important;
}

/* Aumenta largura e altura do calendário */
:deep(.calendario-wrapper .vuecal) {
  width: 20vw;
  height: 43vh;
}
:deep(.vuecal__views-bar) {
  display: none !important; /* Esconde a barra superior com o título e navegação */
}

:deep(.vuecal__nav--today) {
  display: none !important; /* Esconde o botão 'Hoje' */
}
</style>
