<template>
  <Header />
  <BackButton class="voltar" />
  <div class="d-flex justify-content-center align-items-center min-vh-100">
    <div class="form">
      <div class="title mb-4">
        <h1>Listagem de Pacientes</h1>
      </div>

      <div id="form-paciente" class="d-flex gap-3 align-items-center mb-3">
        <input
          type="search"
          class="form-control"
          id="procura-paciente"
          placeholder="Digite o nome ou CPF do paciente"
          v-model="paciente"
          @input="procurarPaciente"
        />
        <RouterLink
          to="/cadastroPaciente"
          class="btn btn-primary d-flex align-items-center gap-2"
        >
          <i class="bi bi-plus-lg"></i>
        </RouterLink>
      </div>

      <span v-if="error !== ''" class="alert alert-danger d-block">{{
        error
      }}</span>

      <div
        id="pacientes-data"
        v-if="pacientes.length"
        class="d-flex flex-column gap-1"
      >
        <div
          v-for="(pacienteInfo, idx) in pacientes"
          :key="pacienteInfo.id || idx"
          class="paciente card p-2 align-items-start shadow-sm"          
        >
          <RouterLink
            @click="verPerfilPaciente(pacienteInfo)"
            to="/perfilPaciente"
            class="paciente-info"
          >
            {{ pacienteInfo.nome }}
          </RouterLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import BackButton from "@/components/btnVoltar.vue";
import { useUsuarioStore } from "@/store/usuario";
import Header from "@/components/Header.vue";
import api from "@/services/api";

export default {
  name: "VisualizarPacientes",
  data() {
    return {
      paciente: "",
      pacientes: [],
      error: "",
    };
  },
  components: {
    Header,
    BackButton,
  },
  methods: {
    verPerfilPaciente(usuarioSelecionado) {
      const usuarioStore = useUsuarioStore();

      usuarioStore.setUsuario(usuarioSelecionado);
    },
    capitalizar(str) {
      if (!str) return "";
      return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
    },
    async procurarPaciente() {
      this.error = "";

      try {
        const onlyNumbers = this.paciente.replace(/\D/g, "");
        const isCpf = onlyNumbers.length === 11;

        // Se campo está vazio, lista todos
        if (this.paciente === "") {
          const res = await api.get("/api/Paciente/ListarTodos");
          this.pacientes = res.data.data || [];
          return;
        }

        // CPF incompleto → não busca e não mostra erro
        if (
          onlyNumbers.length > 0 &&
          onlyNumbers.length < 11 &&
          /^\d+$/.test(this.paciente)
        ) {
          this.pacientes = [];
          return;
        }

        // Chama API com filtro
        const res = await api.get("/api/Paciente/ListarComFiltro", {
          params: {
            nome: isCpf ? "" : this.capitalizar(this.paciente),
            cpf: isCpf ? onlyNumbers : "",
          },
        });

        this.pacientes = res.data.data || [];

        // Só mostra erro se busca foi realmente feita e não encontrou ninguém
        if (!this.pacientes.length && (isCpf || !/^\d+$/.test(this.paciente))) {
          this.error = "Pacientes não encontrados.";
        }
      } catch (error) {
        this.error = "Erro ao buscar pacientes.";
        this.pacientes = [];
      }
    },
  },
  mounted() {
    this.procurarPaciente();
  },
};
</script>

<style scoped>
.title {
  text-align: center;
}
.title h1 {
  font-size: 1.5rem;
}
#form-paciente {
  width: 100%;
  padding: 10px;
}
.paciente {
  transition: transform 0.3s ease;
  cursor: pointer;
}
.paciente:hover {
  transform: scale(1.01);
  background-color: #f0f8ff;
}
.paciente-info {
  width: 100%;
  height: 100%;
}
.voltar {
  position: absolute;
  top: 7rem;
  left: 1vw;
}
.form {
  padding: 1rem;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
  width: 50%;
  height: 60vh;
  gap: 1rem;
}
#pacientes-data {
  overflow-y: auto;
  padding: 10px; /* para evitar que o conteúdo fique colado com a barra de rolagem */
}
/* Estilizar a barra de rolagem, opcional */
#pacientes-data::-webkit-scrollbar {
  width: 5px;
}
#pacientes-data::-webkit-scrollbar-thumb {
  background-color: #186fc0;
  border-radius: 10px;
}
.btn-primary {
  background-color: #d8bd2c;
  transition: 0.3s ease;
  border: none;
}
.btn-primary:hover {
  background-color: #186fc0;
}
@media (max-width: 850px) {
  .form {
    width: 80%;
  }
  span {
    font-size: smaller;
  }
}
@media (max-width: 768px) {
  .voltar {
    display: none;
  }
}
</style>
