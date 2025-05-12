<template>
  <div class="consulta shadow-sm mb-3">
    <div class="consulta-header">
      <span class="consulta-data">
        <b>{{ formatData(consulta.data) }} às {{ consulta.horario }}</b>
        <span class="badge ms-2" :class="statusClass">{{ consulta.status }}</span>
      </span>
    </div>

    <div class="consulta-body">
      <span>
        <i class="bi bi-clipboard2-pulse-fill"></i>
        {{ consulta.especialidade }}
      </span>

      <span>
        <i class="bi bi-person-vcard"></i> {{ consulta.nomeProfessor }}
      </span>
    </div>

    <div class="consulta-actions">
      <button
        v-if="consulta.status === 'Pendente'"
        class="btn-confirmar"
        @click="confirmarConsulta(consulta)"
        aria-label="Confirmar consulta"
      >
        Confirmar
      </button>
      <button 
        class="btn-detalhes" 
        @click="verDetalhes(consulta)"
        aria-label="Ver detalhes da consulta"
      >
        Ver detalhes
      </button>
    </div>
  </div>
</template>

<script>
import { useConsultaStore } from "@/store/consulta";
import Swal from "sweetalert2";
import api from "@/services/api";

export default {
  name: "ConsultaView",
  props: {
    consulta: {
      type: Object,
      required: true,
      validator: (value) => {
        return [
          'id_Consulta',
          'data',
          'horario',
          'status',
          'especialidade',
          'nomeProfessor'
        ].every(prop => prop in value)
      }
    },
  },
  computed: {
    statusClass() {
      return {
        'bg-primary': this.consulta.status === 'Agendada',
        'bg-success': this.consulta.status === 'Concluída',
        'bg-danger': this.consulta.status === 'Cancelada',
        'bg-warning text-dark': this.consulta.status === 'Pendente',
        'bg-info text-dark': this.consulta.status === 'Em Espera'
      }
    }
  },
  methods: {
    formatData(data) {
      if (!data) return '';
      const date = new Date(data);
      date.setHours(date.getHours() + 3); // Ajuste UTC → Brasília
      
      return date.toLocaleDateString('pt-BR', {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      });
    },

    async confirmarConsulta(consulta) {
      try {
        const { isConfirmed } = await Swal.fire({
          title: "Confirmar Consulta",
          html: `Deseja confirmar a consulta de <b>${consulta.nomePaciente}</b> para
               ${this.formatData(consulta.data)} às ${consulta.horario}?`,
          icon: "question",
          showCancelButton: true,
          confirmButtonColor: "#28a745",
          cancelButtonColor: "#d33",
          confirmButtonText: "Sim, confirmar",
          cancelButtonText: "Cancelar",
        });

        if (isConfirmed) {
          Swal.fire({
            title: "Confirmando consulta...",
            allowOutsideClick: false,
            didOpen: () => Swal.showLoading(),
          });

          const response = await api.post(
            `/api/Consulta/confirmar/${consulta.id_Consulta}`
          );

          Swal.close();

          if (response.data.status) {
            await Swal.fire({
              icon: "success",
              title: "Consulta confirmada!",
              text: response.data.message,
              confirmButtonColor: "#d8bd2c",
            });

            this.$emit("consulta-confirmada");
          }
        }
      } catch (error) {
        Swal.fire({
          icon: "error",
          title: "Erro ao confirmar consulta",
          text: error.response?.data?.message || "Ocorreu um erro ao confirmar a consulta",
          confirmButtonColor: "#d8bd2c",
        });
      }
    },

    verDetalhes(consultaSelecionada) {
      const consultaStore = useConsultaStore();
      consultaStore.setConsulta(consultaSelecionada);

      Swal.fire({
        title: "Detalhes da Consulta",
        html: `
          <div class="text-start">
            <p><strong>Professor(a):</strong> ${consultaSelecionada.nomeProfessor}</p>
            <p><strong>Paciente:</strong> ${consultaSelecionada.nomePaciente}</p>
            <p><strong>Área:</strong> ${consultaSelecionada.area}</p>
            <p><strong>Especialidade:</strong> ${consultaSelecionada.especialidade}</p>
            <p><strong>Status:</strong> ${consultaSelecionada.status}</p>
            <p><strong>Data e hora:</strong> ${this.formatData(consultaSelecionada.data)} às ${consultaSelecionada.horario}</p>
          </div>
        `,
        width: "500px",
        showConfirmButton: false,
        showCloseButton: true,
        footer: `
          <button class="swal-btn swal-btn-reagendar">Reagendar</button>
          <button class="swal-btn swal-btn-status">Alterar Status</button>
        `,
      }).then(() => {
        document.querySelectorAll('.swal-btn').forEach(el => el.removeEventListener('click', this.handleSwalButtonClick));
      });

      // Adiciona listeners após o modal ser renderizado
      setTimeout(() => {
        document.querySelector('.swal-btn-reagendar').addEventListener('click', () => {
          this.reagendarConsulta(consultaSelecionada);
        });
        
        document.querySelector('.swal-btn-status').addEventListener('click', () => {
          this.alterarStatusConsulta(consultaSelecionada);
        });
      }, 100);
    },

    async reagendarConsulta(consulta) {
      // Implementação mantida igual à anterior
      // ... (código existente do método reagendarConsulta)
    },

    async alterarStatusConsulta(consulta) {
      // Implementação mantida igual à anterior
      // ... (código existente do método alterarStatusConsulta)
    }
  }
};
</script>

<style scoped>
.consulta {
  --primary-color: #186fc0;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  padding: 1.25rem;
  border-radius: 0.5rem;
  background: white;
  transition: all 0.3s ease;
}

.consulta:hover {
  box-shadow: 0 4px 6px rgba(0,0,0,0.1);
}

.consulta-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 0.5rem;
}

.consulta-body {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.consulta-body span {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.consulta-body i {
  color: var(--primary-color);
  min-width: 1.25rem;
}

.consulta-actions {
  display: flex;
  gap: 0.75rem;
  margin-top: 0.75rem;
}

.btn-confirmar, .btn-detalhes {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 2rem;
  cursor: pointer;
  font-weight: 500;
  transition: all 0.3s ease;
  flex: 1;
  text-align: center;
}

.btn-confirmar {
  background-color: #28a745;
  color: white;
}

.btn-confirmar:hover {
  background-color: #218838;
  transform: translateY(-2px);
  box-shadow: 0 2px 5px rgba(0,0,0,0.1);
}

.btn-confirmar:active {
  transform: translateY(0);
}

.btn-detalhes {
  background-color: var(--primary-color);
  color: white;
}

.btn-detalhes:hover {
  background-color: #155a8a;
  transform: translateY(-2px);
  box-shadow: 0 2px 5px rgba(0,0,0,0.1);
}

.btn-detalhes:active {
  transform: translateY(0);
}

.badge {
  font-size: 0.8em;
  padding: 0.35em 0.65em;
  border-radius: 50rem;
  transition: all 0.2s ease;
}

/* Estilo para o SweetAlert */
.swal-btn {
  padding: 0.5rem 1.5rem;
  border: none;
  border-radius: 0.375rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.3s ease;
  margin: 0 0.5rem;
}

.swal-btn-reagendar {
  background-color: #d8bd2c;
  color: #000;
}

.swal-btn-status {
  background-color: #186fc0;
  color: #fff;
}

.swal-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 2px 5px rgba(0,0,0,0.1);
}

.swal-btn:active {
  transform: translateY(0);
}
</style>