<template>
  <Header />

  <div class="main d-flex justify-content-center align-items-center">
    <div class="form card">
      <div class="Title">
        <BackButton class="voltar" />
        <h2>Agendar Consulta</h2>
      </div>

      <!-- Formulário em etapas -->
      <form @submit.prevent="criarConsulta">
        <!-- Etapa 1: Seleção de Área e Especialidade -->
        <div v-if="etapaAtual === 1" class="etapa">
          <div class="form-group">
            <label for="area">Área</label>
            <select
              v-model="selectedArea"
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

          <div class="form-group botaocentro">
            <button
              type="button"
              class="btn btn-primary"
              @click="avancarEtapa(2)"
              :disabled="!selectedArea || !selectedEspecialidade"
            >
              Próximo
            </button>
          </div>
        </div>

        <!-- Etapa 2: Seleção de Paciente -->
        <div v-if="etapaAtual === 2" class="etapa">
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
            <label for="anamnese">Anamnese</label>
            <textarea
            id="anamnese"
            placeholder="Escreva um pouco sobre as queixas do paciente"
            class="form-control"
            maxlength="255"
            v-model="anamnese"
            required/>
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

          <div class="botoes-navegacao">
            <button
              type="button"
              class="btn btn-secondary"
              @click="retrocederEtapa(1)"
            >
              Voltar
            </button>
            <button
              type="button"
              class="btn btn-primary"
              @click="avancarEtapa(3)"
              :disabled="!selectedPacienteId || !selectedProfessor || !anamnese"
            >
              Próximo
            </button>
          </div>
        </div>

        <!-- Etapa 3: Seleção de Data e Horário -->
        <div v-if="etapaAtual === 3" class="etapa">
          <div class="calendario-wrapper">
            <vue-cal
              date-picker
              default-view="month"
              :time="false"
              :locale="'pt-br'"
              :min-date="new Date().toISOString().split('T')[0]"
              :disabled-dates="disableSundays"
              @cell-click="selecionarData"
              :selected-date="dataConsulta"
            />
          </div>

          <div class="botoes-navegacao">
            <button
              type="button"
              class="btn btn-secondary"
              @click="retrocederEtapa(2)"
            >
              Voltar
            </button>
            <button
              type="button"
              class="btn btn-primary"
              @click="mostrarHorariosSweetAlert"
              :disabled="!dataConsulta"
            >
              Selecionar Horário
            </button>
          </div>
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
      anamnese: "",
      selectedProfessor: "",
      searchPaciente: "",
      dataConsulta: "",
      horarioConsulta: "",
      statusConsulta: "Pendente",
      erro: null,
      etapaAtual: 1, // Nova propriedade para controlar a etapa atual
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
    formatarData(data) {
      if (!data) return "";

      const date = new Date(data);
      // Adiciona 3 horas para compensar UTC → Brasília
      date.setHours(date.getHours() + 3);

      const options = {
        weekday: "long",
        year: "numeric",
        month: "long",
        day: "numeric",
      };

      return date.toLocaleDateString("pt-BR", options);
    },

    avancarEtapa(novaEtapa) {
      this.etapaAtual = novaEtapa;
      // Quando avançar para a etapa 3, carregar horários se já tiver data selecionada
      if (novaEtapa === 3 && this.dataConsulta) {
        this.obterHorariosDisponiveis();
      }
    },

    retrocederEtapa(etapaAnterior) {
      this.etapaAtual = etapaAnterior;
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
      this.obterHorariosDisponiveis();
    },

    selecionarHorario(horario) {
      this.horarioConsulta = horario;
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

    async mostrarHorariosSweetAlert() {
      try {
        // Mostra loading enquanto busca horários (exatamente como no exemplo)
        Swal.fire({
          title: "Buscando horários...",
          html: "Aguarde enquanto carregamos os horários disponíveis",
          allowOutsideClick: false,
          didOpen: () => {
            Swal.showLoading();
          },
        });

        const response = await api.get("/api/Consulta/horarios-disponiveis", {
          params: {
            data: this.dataConsulta,
            area: this.selectedArea,
            especialidade: this.selectedEspecialidade,
          },
        });

        Swal.close(); // Fecha loading

        // Verifica se existem horários (igual ao exemplo)
        if (!response.data.status || !response.data.data.length) {
          return Swal.fire({
            icon: "error",
            title: "Sem horários disponíveis",
            text: "Não encontramos horários para esta data. Tente outra data ou entre em contato.",
            confirmButtonColor: "#d8bd2c",
          });
        }

        const horariosDisponiveis = response.data.data;

        // Cria as opções de seleção igual ao exemplo
        const inputOptions = horariosDisponiveis.reduce((options, horario) => {
          const group =
            horario.status === "Disponível"
              ? "Horários Disponíveis"
              : "Fila de Espera";
          if (!options[group]) options[group] = {};
          options[group][horario.horario] = `${horario.horario}${
            horario.status !== "Disponível" ? ` (${horario.status})` : ""
          }`;
          return options;
        }, {});

        // Mostra o select de horários igual ao exemplo
        const { value: novoHorario } = await Swal.fire({
          title: `Selecione o horário para ${this.formatarData(
            this.dataConsulta
          )}`,
          input: "select",
          inputOptions: inputOptions,
          inputPlaceholder: "Selecione um horário",
          showCancelButton: true,
          confirmButtonText: "Confirmar",
          cancelButtonText: "Voltar",
          confirmButtonColor: "#d8bd2c",
          inputValidator: (value) =>
            !value && "Selecione um horário para continuar",
        });

        if (novoHorario) {
          // Remove o status do horário se existir (igual ao exemplo)
          this.horarioConsulta = novoHorario.split(" ")[0];

          // Mostra confirmação antes de agendar
          const { isConfirmed } = await Swal.fire({
            title: "Confirmar agendamento?",
            html: `
            <div style="text-align:left">
              <p><b>Detalhes do agendamento:</b></p>
              <p>• Data: <b>${this.formatarData(this.dataConsulta)}</b></p>
              <p>• Horário: <b>${this.horarioConsulta}</b></p>
              <p>• Área: <b>${this.selectedArea}</b></p>
              <p>• Especialidade: <b>${this.selectedEspecialidade}</b></p>
            </div>
          `,
            icon: "question",
            showCancelButton: true,
            confirmButtonColor: "#28a745",
            cancelButtonColor: "#d33",
            confirmButtonText: "Confirmar Agendamento",
            cancelButtonText: "Revisar",
          });

          if (isConfirmed) {
            // Chama o método de criação da consulta
            await this.criarConsulta();
          }
        }
      } catch (error) {
        Swal.close();
        console.error("Erro ao carregar horários:", error);

        await Swal.fire({
          icon: "error",
          title: "Erro ao carregar horários",
          text: "Ocorreu um erro ao buscar os horários disponíveis. Por favor, tente novamente.",
          confirmButtonColor: "#d8bd2c",
        });
      }
    },
    // Modificação no método obterHorariosDisponiveis para carregar automaticamente
    async obterHorariosDisponiveis() {
      if (
        !this.dataConsulta ||
        !this.selectedArea ||
        !this.selectedEspecialidade
      )
        return;

      try {
        this.isLoadingHorarios = true;
        const response = await api.get("/api/Consulta/horarios-disponiveis", {
          params: {
            data: this.dataConsulta,
            area: this.selectedArea,
            especialidade: this.selectedEspecialidade,
          },
        });

        if (response.data.status) {
          this.horariosDisponiveis = response.data.data;
        } else {
          this.horariosDisponiveis = [];
          // Mostrar feedback apenas se estiver na etapa de seleção de horário
          if (this.etapaAtual === 3) {
            Swal.fire({
              icon: "info",
              title: "Nenhum horário disponível",
              text: response.data.message,
              confirmButtonColor: "#d8bd2c",
            });
          }
        }
      } catch (error) {
        console.error("Erro ao carregar horários disponíveis:", error);
        if (this.etapaAtual === 3) {
          this.erro = "Erro ao carregar horários. Tente novamente mais tarde.";
        }
      } finally {
        this.isLoadingHorarios = false;
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
        anamnese: this.anamnese,
        pacienteId: this.selectedPacienteId,
        professorId: this.selectedProfessor,
        dataCadastro: new Date().toISOString(), // Adiciona a data de cadastro atual
      };

      // Armazena a referência do alerta de loading
      const loadingAlert = Swal.fire({
        title: "Agendando consulta...",
        html: "Por favor, aguarde enquanto processamos seu agendamento",
        allowOutsideClick: false,
        didOpen: () => {
          Swal.showLoading();
        },
      });

      try {
        const response = await api.post(
          "/api/Consulta/CreateConsulta",
          consulta
        );

        // Fecha o loading antes de mostrar o sucesso
        await loadingAlert.close();

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

        this.$router.push("/admin");
      } catch (error) {
        // Fecha o loading em caso de erro também
        await loadingAlert.close();

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
#anamnese {
  max-height: 110px
}
.form {
  padding: 2rem;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  width: 100%;
  max-width: 900px;
  gap: 1rem;
}
.consulta {
  display: grid;
  grid-template-columns: repeat(2, 1fr); /* 2 colunas com largura igual */
  gap: 1rem; /* Espaçamento entre os itens */
}
.main {
  margin-top: 15vh;
}

.form-group {
  margin-bottom: 1.5rem;
}

.botaocentro {
  margin-bottom: 1rem;
}

.Title {
  text-align: center;
  display: grid;
  grid-template-columns: auto 1fr; /* 2 colunas: uma para a seta e outra para o título */
  align-items: center; /* Alinha os itens verticalmente */
}

.Title h2 {
  text-align: center;
  font-weight: 400;
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

.calendario-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
}

/* Novos estilos para o fluxo em etapas */
.etapa {
  padding: 1rem;
  animation: fadeIn 0.3s ease;
}

@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.botoes-navegacao {
  display: flex;
  justify-content: space-between;
  margin-top: 2rem;
}

/* Estilos para a grade de horários */
.horarios-container {
  margin-top: 2rem;
  animation: slideUp 0.4s ease;
}

.horarios-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  gap: 0.8rem;
  margin-top: 1rem;
}

.horario-btn {
  padding: 0.8rem;
  border: 1px solid #ddd;
  border-radius: 8px;
  background: white;
  cursor: pointer;
  transition: all 0.2s ease;
  position: relative;
  text-align: center;
}

.horario-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.horario-disponivel {
  background-color: #f8f9fa;
  border-color: #186fc0;
  color: #186fc0;
}

.horario-disponivel:hover {
  background-color: #e7f3ff;
}

.horario-indisponivel {
  background-color: #f8f9fa;
  border-color: #ddd;
  color: #6c757d;
  cursor: not-allowed;
  opacity: 0.7;
}

.horario-selecionado {
  background-color: #d8bd2c;
  color: white;
  border-color: #d8bd2c;
}

.badge {
  position: absolute;
  top: -8px;
  right: -8px;
  background-color: #dc3545;
  color: white;
  border-radius: 50%;
  width: 20px;
  height: 20px;
  font-size: 0.7rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Estilos para o SweetAlert de horários */
.swal-horarios-container {
  max-height: 60vh;
  overflow-y: auto;
  padding: 10px;
}

.swal-horarios-grid {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.swal-horarios-group h5 {
  text-align: left;
  margin: 0 0 10px 0;
  color: #186fc0;
  font-size: 1.1rem;
}

.swal-horarios-buttons {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(100px, 1fr));
  gap: 10px;
}

.swal-horario-btn {
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 8px;
  background: white;
  cursor: pointer;
  transition: all 0.2s ease;
  position: relative;
  text-align: center;
  font-size: 14px;
  outline: none;
}

.swal-horario-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.swal-horario-btn:not(.espera) {
  border-color: #186fc0;
  color: #186fc0;
}

.swal-horario-btn:not(.espera):hover {
  background-color: #e7f3ff;
}

.swal-horario-btn.selected {
  background-color: #d8bd2c;
  color: white;
  border-color: #d8bd2c;
}

.swal-horario-btn.espera {
  border-color: #ddd;
  color: #6c757d;
  opacity: 0.9;
}

.swal-horario-btn.espera:hover {
  opacity: 1;
}

.swal-badge {
  position: absolute;
  top: -8px;
  right: -8px;
  background-color: #dc3545;
  color: white;
  border-radius: 50%;
  width: 18px;
  height: 18px;
  font-size: 0.6rem;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Ajustes para mobile */
@media (max-width: 600px) {
  .swal-horarios-buttons {
    grid-template-columns: repeat(auto-fill, minmax(80px, 1fr));
  }

  .swal-horarios-group h5 {
    font-size: 1rem;
  }
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
  width: 400px;
  height: 43vh;
}
:deep(.vuecal__views-bar) {
  display: none !important; /* Esconde a barra superior com o título e navegação */
}

:deep(.vuecal__nav--today) {
  display: none !important; /* Esconde o botão 'Hoje' */
}

@media (max-width: 1000px) {
  :deep(.calendario-wrapper .vuecal) {
    width: 40vw;
  }
}

@media (max-width: 400px) {
  .consulta {
    display: grid;
    grid-template-columns: repeat(1, 1fr); /* 2 colunas com largura igual */
    gap: 1rem; /* Espaçamento entre os itens */
  }

  :deep(.calendario-wrapper .vuecal) {
    width: 80vw;
  }

  .voltar {
    display: none;
  }
  .main {
    margin-top: 10vh;
  }
}
</style>
