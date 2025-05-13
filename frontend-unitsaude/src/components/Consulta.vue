<template>
  <div class="consulta shadow-sm mb-1">
    <span>
      <b>{{ formatData(consulta.data) }} às {{ consulta.horario }}</b>

      <span v-if="consulta.status === 'Agendada'" class="badge bg-primary ms-2"
        >Agendada</span
      >
      <span v-if="consulta.status === 'Concluída'" class="badge bg-success ms-2"
        >Concluída</span
      >
      <span v-if="consulta.status === 'Cancelada'" class="badge bg-danger ms-2"
        >Cancelada</span
      >
      <span v-if="consulta.status === 'Pendente'" class="badge bg-warning ms-2"
        >Pendente</span
      >
      <span
        v-else-if="consulta.status === 'Em Espera'"
        class="badge bg-info ms-2"
        >Em Espera</span
      >
    </span>
    <span>
      <i class="bi bi-clipboard2-pulse-fill"></i>
      {{ consulta.especialidade }}
    </span>
    <span>
      <i class="bi bi-person-vcard"></i> {{ consulta.nomeProfessor }}
    </span>

    <div class="consulta-detalhes">
      <button
        v-if="consulta.status === 'Pendente' && isAdmin"
        class="btn-confirmar me-2"
        @click="confirmarConsulta(consulta)"
      >
        Confirmar
      </button>
      <button class="btn-consulta" @click="verDetalhes(consulta)">
        Ver detalhes
      </button>
    </div>
  </div>
</template>

<script>
import { useConsultaStore } from "@/store/consulta";
import Swal from "sweetalert2";
import api from "@/services/api";
import { useAuthStore } from "@/store/auth";

export default {
  name: "ConsultaItem",
  props: {
    consulta: {
      type: Object,
      required: true,
    },
  },
  data() {
    return {
      auth: useAuthStore(),
    };
  },

  methods: {
    formatData(data) {
      if (!data) return "";
      const date = new Date(data);
      date.setHours(date.getHours() + 3); // Ajuste para fuso horário

      return date.toLocaleDateString("pt-BR", {
        weekday: "long",
        year: "numeric",
        month: "long",
        day: "numeric",
      });
    },

    async verDetalhes(consulta) {
      const consultaStore = useConsultaStore();
      consultaStore.setConsulta(consulta);

      const { isAdmin } = this;
      const buttons = isAdmin
        ? {
            showDenyButton: true,
            denyButtonText: "Alterar Status",
            showCancelButton: true,
            cancelButtonText: "Reagendar",
          }
        : {};

      const result = await Swal.fire({
        title: "Detalhes da Consulta",
        html: this.getConsultaDetailsHtml(consulta),
        icon: "info",
        width: "500px",
        showConfirmButton: false,
        showCloseButton: true,
        ...buttons,
        customClass: {
          popup: "consulta-popup",
        },
      });

      if (isAdmin) {
        if (result.isDenied) {
          await this.alterarStatusConsulta(consulta);
        } else if (
          result.isDismissed &&
          result.dismiss === Swal.DismissReason.cancel
        ) {
          await this.reagendarConsulta(consulta);
        }
      }
    },

    getConsultaDetailsHtml(consulta) {
      return `
        <div class="text-start">
          <p><strong>Professor(a):</strong> ${consulta.nomeProfessor}</p>
          <p><strong>Paciente:</strong> ${consulta.nomePaciente}</p>
          <p><strong>Área:</strong> ${consulta.area}</p>
          <p><strong>Especialidade:</strong> ${consulta.especialidade}</p>
          <p><strong>Status:</strong> ${consulta.status}</p>
          <p><strong>Data e hora:</strong> ${this.formatData(
            consulta.data
          )} às ${consulta.horario}</p>
        </div>
      `;
    },

    async confirmarConsulta(consulta) {
      try {
        const { isConfirmed } = await Swal.fire({
          title: "Confirmar Consulta",
          html: `Deseja confirmar a consulta de <b>${
            consulta.nomePaciente
          }</b> para
               ${this.formatData(consulta.data)} às ${consulta.horario}?`,
          icon: "question",
          showCancelButton: true,
          confirmButtonColor: "#28a745",
          cancelButtonColor: "#d33",
          confirmButtonText: "Sim, confirmar",
          cancelButtonText: "Cancelar",
        });

        if (!isConfirmed) return;

        Swal.fire({
          title: "Confirmando...",
          allowOutsideClick: false,
          didOpen: () => Swal.showLoading(),
        });

        const { data } = await api.post(
          `/api/Consulta/confirmar/${consulta.id_Consulta}`
        );

        Swal.fire({
          icon: "success",
          title: "✅ Confirmada!",
          text: data.message,
          confirmButtonColor: "#d8bd2c",
        });

        this.$emit("consulta-confirmada", {
          id: consulta.id_Consulta,
          novoStatus: "Agendada",
        });
      } catch (error) {
        this.handleError(error, "Erro ao confirmar consulta");
      }
    },

    async reagendarConsulta(consulta) {
      try {
        const validateDate = (dateString) => {
          const date = new Date(dateString);
          const today = new Date();
          today.setHours(0, 0, 0, 0);

          // Valida se é domingo (0 = Domingo, 1 = Segunda, etc.)
          if (date.getDay() === 0) {
            return {
              isValid: false,
              message: "Domingos não estão disponíveis",
            };
          }

          // Valida se é data passada
          if (date < today) {
            return {
              isValid: false,
              message: "Não é possível selecionar datas passadas",
            };
          }

          return { isValid: true };
        };

        // 1. ETAPA - SELECIONAR NOVA DATA
        let dateSelected = false;
        let validDate = false;

        while (!dateSelected || !validDate) {
          const { value: novaData, isDismissed } = await Swal.fire({
            title: "Selecione a nova data",
            input: "date",
            inputAttributes: {
              min: new Date().toISOString().split("T")[0],
              style:
                "width: 50%; margin-left: auto; margin-right: auto; display: block !important;",
            },
            showCancelButton: true,
            confirmButtonText: "Próximo",
            cancelButtonText: "Cancelar",
            confirmButtonColor: "#d8bd2c",
            cancelButtonColor: "#d33",
            preConfirm: (date) => {
              if (!date) {
                Swal.showValidationMessage("Selecione uma data válida");
                return false;
              }

              const validation = validateDate(date);
              if (!validation.isValid) {
                Swal.showValidationMessage(validation.message);
                return false;
              }

              return date;
            },
            didOpen: () => {
              const dateInput = document.querySelector(
                '.swal2-input[type="date"]'
              );
              if (dateInput) {
                dateInput.style.display = "block";
                dateInput.style.width = "50%";
                dateInput.style.marginLeft = "auto";
                dateInput.style.marginRight = "auto";

                // Bloqueio imediato de domingos
                dateInput.addEventListener("change", function () {
                  if (this.value) {
                    const day = new Date(this.value).getDay();
                    if (day === 0) {
                      this.value = "";
                      Swal.showValidationMessage(
                        "Domingos não estão disponíveis"
                      );
                    }
                  }
                });
              }
            },
          });

          if (isDismissed) return; // Usuário cancelou

          dateSelected = true;
          validDate = novaData !== false;

          if (!validDate) continue; // Volta para seleção de data se inválida

          // 2. ETAPA - BUSCAR HORÁRIOS DISPONÍVEIS (COM LOADING)
          try {
            // Mostra loading enquanto busca horários
            Swal.fire({
              title: "Buscando horários...",
              html: "Aguarde enquanto carregamos os horários disponíveis",
              allowOutsideClick: false,
              didOpen: () => {
                Swal.showLoading();
              },
            });

            const responseHorarios = await api.get(
              "/api/Consulta/horarios-disponiveis",
              {
                params: {
                  data: novaData,
                  area: consulta.area,
                  especialidade: consulta.especialidade,
                  // Adicione se necessário: professorId: consulta.professorId
                },
              }
            );

            Swal.close(); // Fecha loading

            // Verifica se existem horários
            if (
              !responseHorarios.data.status ||
              !responseHorarios.data.data.length
            ) {
              return Swal.fire({
                icon: "error",
                title: "Sem horários disponíveis",
                text: "Não encontramos horários para esta data. Tente outra data ou entre em contato.",
                confirmButtonColor: "#d8bd2c",
              });
            }

            const horariosDisponiveis = responseHorarios.data.data;

            // 3. ETAPA - SELECIONAR HORÁRIO
            const { value: novoHorario } = await Swal.fire({
              title: `Selecione o horário para ${this.formatData(novaData)}`,
              input: "select",
              inputOptions: horariosDisponiveis.reduce((options, horario) => {
                const group =
                  horario.status === "Disponível"
                    ? "Horários Disponíveis"
                    : "Fila de Espera";
                if (!options[group]) options[group] = {};
                options[group][horario.horario] = `${horario.horario}${
                  horario.status !== "Disponível" ? ` (${horario.status})` : ""
                }`;
                return options;
              }, {}),
              inputPlaceholder: "Selecione um horário",
              showCancelButton: true,
              confirmButtonText: "Reservar",
              cancelButtonText: "Voltar",
              confirmButtonColor: "#d8bd2c",
              inputValidator: (value) =>
                !value && "Selecione um horário para continuar",
            });

            if (!novoHorario) return this.reagendarConsulta(consulta); // Volta para o início se cancelar

            // 4. ETAPA - CONFIRMAÇÃO (COM LOADING DURANTE PROCESSAMENTO)
            const { isConfirmed } = await Swal.fire({
              title: "Confirmar reagendamento?",
              html: `<div style="text-align:left">
          <p><b>Detalhes do reagendamento:</b></p>
          <p>• Data: <b>${this.formatData(novaData)}</b></p>
          <p>• Horário: <b>${novoHorario.split(" ")[0]}</b></p>
          <p>• Local: Centro Universitário Tiradentes - UNIT PE</p>
        </div>`,
              icon: "question",
              showCancelButton: true,
              confirmButtonColor: "#28a745",
              cancelButtonColor: "#d33",
              confirmButtonText: "Confirmar Reagendamento",
              cancelButtonText: "Revisar",
            });

            if (!isConfirmed) return this.reagendarConsulta(consulta);

            // PROCESSAR REAGENDAMENTO (COM LOADING)
            Swal.fire({
              title: "Processando...",
              html: "Estamos atualizando seu agendamento",
              allowOutsideClick: false,
              didOpen: () => {
                Swal.showLoading();
              },
            });

            const resposta = await api.patch(
              `/api/Consulta/ReagendarConsulta/${consulta.id_Consulta}`,
              {
                novaData: novaData,
                novoHorario: novoHorario.split(" ")[0], // Remove status se existir
              }
            );

            Swal.close(); // Fecha loading

            // 5. RESULTADO
            if (resposta.data.status) {
              await Swal.fire({
                icon: "success",
                title: "✅ Reagendado!",
                html: `<div style="text-align:left">
            <p>Sua consulta foi reagendada com sucesso:</p>
            <p><b>${this.formatData(novaData)} às ${
                  novoHorario.split(" ")[0]
                }</b></p>
            <p>Você receberá um e-mail de confirmação.</p>
          </div>`,
                confirmButtonColor: "#d8bd2c",
                willClose: () => {
                  // Atualize a lista de consultas se necessário
                  this.$emit("consulta-reagendada");
                },
              });

              // Dentro de reagendarConsulta():
              this.$emit("consulta-atualizada", {
                id: consulta.id_Consulta,
                novoStatus: consulta.status, // Mantém ou atualiza conforme o backend
              });
            } else {
              throw new Error(resposta.data.message || "Erro desconhecido");
            }
          } catch (error) {
            Swal.close();
            console.error("Erro no reagendamento:", error);

            await Swal.fire({
              icon: "error",
              title: "Falha no reagendamento",
              html: `<div style="text-align:left">
          <p>Não foi possível completar o reagendamento:</p>
          <p><b>${error.response?.data?.message || error.message}</b></p>
          <p>Tente novamente ou entre em contato com o suporte.</p>
        </div>`,
              confirmButtonColor: "#d8bd2c",
            });

            // Permite tentar novamente
            this.reagendarConsulta(consulta);
          }
        }
      } catch (error) {
        this.handleError(error, "Erro no reagendamento");
      }
    },

    async alterarStatusConsulta(consulta) {
      try {
        const { value: novoStatus } = await Swal.fire({
          title: "Alterar Status",
          html: this.getStatusChangeHtml(consulta),
          input: "select",
          inputOptions: {
            Agendada: "Agendada",
            Concluída: "Concluída",
            Cancelada: "Cancelada",
          },
          inputPlaceholder: "Selecione o novo status",
          inputValidator: (value) => {
            if (!value) return "Selecione um status válido";
            if (value === consulta.status) return "O status deve ser diferente";
          },
          showCancelButton: true,
          confirmButtonText: "Confirmar",
          cancelButtonText: "Cancelar",
          confirmButtonColor: "#d8bd2c",
          cancelButtonColor: "#d33",
        });

        if (!novoStatus) return;

        Swal.fire({
          title: "Atualizando...",
          allowOutsideClick: false,
          didOpen: () => Swal.showLoading(),
        });

        const { data } = await api.patch(
          `/api/Consulta/${consulta.id_Consulta}/status`,
          {
            novoStatus,
          }
        );

        Swal.fire({
          icon: "success",
          title: "✅ Status atualizado!",
          html: this.getStatusUpdatedHtml(novoStatus),
          confirmButtonColor: "#d8bd2c",
        });

        this.$emit("consulta-atualizada", {
          id: consulta.id_Consulta,
          novoStatus,
        });

        if (novoStatus === "Concluída") {
          await this.verificarConsultasEmEspera(consulta);
        }
      } catch (error) {
        this.handleError(error, "Falha na atualização");
      }
    },

    handleError(error, title) {
      console.error(title, error);
      Swal.fire({
        icon: "error",
        title,
        text: error.response?.data?.message || error.message,
        confirmButtonColor: "#d8bd2c",
      });
    },

    // ... manter os outros métodos (verificarConsultasEmEspera, etc)
  },
  computed: {
    isAdmin() {
      return this.auth.tipoUsuario === "Administrador";
    },
  },
};
</script>

<style scoped>
/* Manter seus estilos existentes */
.consulta {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  padding: 20px;
  gap: 20px;
  min-width: 100%;
  border-radius: 8px;
  background-color: white;
}

.consulta span {
  display: flex;
  align-items: center;
  gap: 10px;
}

.consulta span i {
  color: #186fc0;
}

.consulta-detalhes {
  display: flex;
  margin-top: 10px;
  min-width: 100%;
  gap: 10px;
}

.btn-consulta {
  background-color: #186fc0;
  color: white;
  border: none;
  padding: 5px 25px;
  border-radius: 25px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-consulta:hover {
  background-color: #155a8a;
  transform: scale(1.05);
}

.btn-confirmar {
  background-color: #28a745;
  color: white;
  border: none;
  padding: 5px 15px;
  border-radius: 25px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-confirmar:hover {
  background-color: #218838;
  transform: scale(1.05);
}
</style>
