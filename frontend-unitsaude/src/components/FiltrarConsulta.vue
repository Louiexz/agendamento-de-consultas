<template>
	<!-- Filtros -->
    <div class="filtros mb-4 p-3 bg-light rounded">
      <div class="row g-2 align-items-end">
        <div class="col-md-3">
          <label class="form-label small">Status</label>
          <select class="form-select" v-model="filtros.status">
            <option value="">Todos</option>
            <option v-for="status in opcoesStatus" :key="status" :value="status">
              {{ status }}
            </option>
          </select>
        </div>

        <div class="col-md-3">
          <label class="form-label small">Especialidade</label>
          <select class="form-select" v-model="filtros.especialidade">
            <option value="">Todas</option>
            <option v-for="esp in especialidades" :key="esp" :value="esp">
              {{ esp }}
            </option>
          </select>
        </div>

        <div class="col-md-3">
          <label class="form-label small">Professor</label>
          <select v-if="!isProfessor.status" class="form-select" v-model="filtros.professorId">
		    <option value="">Todos</option>
		    <option v-for="prof in professores" :key="prof.id_Usuario" :value="prof.id_Usuario">
		      {{ prof.nome }}
		    </option>
		  </select>

		  <select v-else class="form-select" v-model="filtros.professorId" disabled>
		    <option :value="filterProfessorId">
		      {{ isProfessor.name }}
		    </option>
		  </select>
        </div>

        <div class="col-md-3">
          <button 
            class="btn-limpar-filtros" 
            @click="limparFiltros"
            :disabled="!filtrosAtivos"
          >
            <i class="bi bi-x-circle"></i> Limpar
          </button>
        </div>
      </div>
    </div>

    <!-- Listagem com barra de rolagem -->
    <div class="consultas-container">
      <div class="d-flex justify-content-between align-items-center mb-3">
        <h4 class="mb-0">Listagem de Consultas</h4>
        <span class="badge bg-secondary">
          {{ consultasFiltradas.length }} registros
        </span>
      </div>

      <div v-if="isLoading" class="text-center my-5">
        <div class="spinner-border text-primary" role="status"></div>
        <p class="mt-2">Carregando...</p>
      </div>

      <div v-else class="lista-consultas">
        <ConsultaView
          v-for="consulta in consultasOrdenadas"
          :key="consulta.id_Consulta"
          :consulta="consulta"
          @consulta-confirmada="handleConsultaConfirmada"
          @consulta-atualizada="getConsultas"
          class="mb-3"
        />

        <div v-if="!consultasFiltradas.length" class="text-center py-4 text-muted">
          <i class="bi bi-calendar-x fs-4"></i>
          <p class="mt-2">Nenhuma consulta encontrada</p>
        </div>
      </div>
    </div>
</template>

<script>
import api from "@/services/api";
import ConsultaView from "@/components/Consulta";

export default {
  props: {
    area: {
      type: String,
      required: true,
      default: ""
    },
    isProfessor: {
      type: Object,
      required: true,
      default: {
      	status: false,
      	name: "",
      	id: 0
      }
    }
  },
  components: {
    ConsultaView,
  },
  data() {
    return {
      consultas: [],
      isLoading: false,
      especialidades: [],
      professores: [],
      opcoesStatus: ['Agendada', 'Pendente', 'Em Espera', 'Concluída', 'Cancelada'],
      filtros: {
        status: '',
        especialidade: '',
        professorId: ''
      }
    };
  },
  computed: {
  	filterProfessorId() {
  	  const prof = this.professores.find(p => p.id_Usuario === this.isProfessor.id);
  	  return prof ? prof.id_Usuario : '';
    },
    filtrosAtivos() {
      return this.filtros.status || this.filtros.especialidade || this.filtros.professorId;
    },
    consultasFiltradas() {
      return this.consultas.filter(c => {
        return (
          c.area === this.area &&
          (!this.filtros.status || c.status === this.filtros.status) &&
          (!this.filtros.especialidade || c.especialidade === this.filtros.especialidade) &&
          (!this.filtros.professorId || c.professorId == this.filtros.professorId)
        );
      });
    },
    consultasOrdenadas() {
      return [...this.consultasFiltradas].sort((a, b) => {
        if (a.status === 'Em Espera' && b.status === 'Em Espera') {
          return new Date(a.dataCadastro) - new Date(b.dataCadastro);
        }
        const dateA = new Date(a.data);
        const dateB = new Date(b.data);
        return dateA - dateB || a.horario.localeCompare(b.horario);
      });
    }
  },
  methods: {
    async getConsultas() {
      this.isLoading = true;
      try {
        if (this.isProfessor.status) {
          const response = await api.get(`api/Consulta/GetConsultaPorProfessor/${this.isProfessor.professorId}`);

          if (response.data?.data) {
            this.consultas = response.data.data;
          }
        } else {
          response = await api.get("api/Consulta/FiltrarConsultas", {
            params: { Area: this.area }
          });
          if (response.data?.data) {
            this.consultas = response.data.data;
            this.carregarProfessores();

            // Se for professor, filtra automaticamente após carregar professores
            if (this.isProfessor.status) {
              const prof = this.professores.find(p => p.nome === this.isProfessor.name);
              if (prof) {
                this.filtros.professorId = prof.id_Usuario;
              }
            }
          }
        }

        
      } catch (error) {
        console.log(error);
      } finally {
        this.isLoading = false;
      }
    },

    async getEspecialidades() {
      const response = await api.get(`api/Consulta/especialidades/${this.area}`);
      if (response.data?.data) {
        this.especialidades = response.data.data;
      }
    },

    carregarProfessores() {
      const professoresMap = {};
      this.consultas.forEach(c => {
        if (c.professorId && !professoresMap[c.professorId]) {
          professoresMap[c.professorId] = {
            id_Usuario: c.professorId,
            nome: c.nomeProfessor
          };
        }
      });
      this.professores = Object.values(professoresMap);
    },

    limparFiltros() {
      this.filtros = {
        status: '',
        especialidade: '',
        professorId: ''
      };
    },

    handleConsultaConfirmada() {
      this.getConsultas();
    }
  },
  async mounted() {
	  await Promise.all([
	    this.getEspecialidades(),
	    this.getConsultas()
	  ]);
	}
};
</script>

<style scoped>
.container {
  margin-top: 6rem;
  margin-bottom: 2rem;
}

.title {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.filtros {
  border: 1px solid #dee2e6;
  transition: border-color 0.3s ease;
}

.filtros:hover {
  border-color: #adb5bd;
}

.consultas-container {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  transition: box-shadow 0.3s ease;
}

.consultas-container:hover {
  box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.1);
}

.lista-consultas {
  max-height: 60vh;
  overflow-y: auto;
  padding-right: 10px;
  scrollbar-width: thin;
  scrollbar-color: #adb5bd #f1f1f1;
}

/* Estilização da barra de rolagem com transições */
.lista-consultas::-webkit-scrollbar {
  width: 10px;
  transition: all 0.3s ease;
}

.lista-consultas::-webkit-scrollbar-track {
  background: #f1f1f1;
  border-radius: 10px;
  transition: background-color 0.3s ease;
}

.lista-consultas::-webkit-scrollbar-thumb {
  background: #adb5bd;
  border-radius: 10px;
  transition: all 0.3s ease;
}

.lista-consultas::-webkit-scrollbar-thumb:hover {
  background: #6c757d;
  transform: scale(1.05);
}

.lista-consultas::-webkit-scrollbar-thumb:active {
  background: #495057;
}

/* Botão Limpar com transições */
.btn-limpar-filtros {
  width: 100%;
  padding: 0.375rem 0.75rem;
  border: 1px solid #dc3545;
  background-color: transparent;
  color: #dc3545;
  border-radius: 0.375rem;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.btn-limpar-filtros:not(:disabled):hover {
  background-color: #dc3545;
  color: white;
  transform: translateY(-1px);
  box-shadow: 0 2px 5px rgba(220, 53, 69, 0.3);
}

.btn-limpar-filtros:not(:disabled):active {
  transform: translateY(0);
  box-shadow: none;
}

.btn-limpar-filtros:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  border-color: #adb5bd;
  color: #adb5bd;
}

@media (max-width: 768px) {
  .container {
    margin-top: 5rem;
    padding: 1rem;
  }
  
  .filtros .row > div {
    margin-bottom: 1rem;
  }
  
  .lista-consultas {
    max-height: 50vh;
  }
  
  .btn-limpar-filtros {
    padding: 0.25rem 0.5rem;
    font-size: 0.875rem;
  }
}
</style>