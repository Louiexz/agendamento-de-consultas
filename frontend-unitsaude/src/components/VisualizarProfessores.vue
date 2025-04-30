<template>
  <Header />
  <BackButton class="voltar" />
  <div class="d-flex justify-content-center align-items-center min-vh-100">
    <div class="form">
      <div class="title mb-4">
        <h1>Listagem de Professores</h1>
      </div>

      <div id="form-professor" class="d-flex gap-3 align-items-center mb-3">
        <input
          type="search"
          class="form-control"
          id="procura-professor"
          placeholder="Digite o nome ou especialidade do professor"
          v-model="professor"
          @input="procurarProfessores"
        />
        <RouterLink
          to="/cadastroProfessor"
          class="btn btn-primary d-flex align-items-center gap-2"
        >
          <i class="bi bi-plus-lg"></i>
        </RouterLink>
      </div>

      <span v-if="error !== ''" class="alert alert-danger d-block">{{
        error
      }}</span>

      <div
        id="professores-data"
        v-if="professores.length"
        class="d-flex flex-column gap-1"
      >
        <div
          v-for="(professorInfo, idx) in professores"
          :key="professorInfo.id || idx"
          class="professor card p-2 align-items-start shadow-sm"
        >
          <span
            >{{ professorInfo.nome }} {{ professorInfo.especialidade }}</span
          >
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import BackButton from "@/components/btnVoltar.vue";
import Header from "@/components/Header.vue";
import api from "@/services/api";

export default {
  name: "VisualizarProfessores",
  data() {
    return {
      professor: "",
      professores: [],
      error: "",
    };
  },
  components: {
    Header,
    BackButton,
  },
  methods: {
    capitalizar(str) {
      if (!str) return "";
      return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
    },
    async procurarProfessores() {
      this.error = "";

      try {
        // Filtra se é CPF ou nome, dependendo do input
        const isEspecialidade = this.professor.includes(" "); // Verifica se tem espaço (assumindo que é especialidade)

        let res;
        if (this.professor === "") {
          res = await api.get("/api/Professor/ListarTodos");
        } else {
          res = await api.get("/api/Professor/ListarComFiltro", {
            params: {
              nome: isEspecialidade ? "" : this.capitalizar(this.professor),
              especialidade: isEspecialidade
                ? this.capitalizar(this.professor)
                : "",
            },
          });
        }

        this.professores = res.data.data || [];

        if (!this.professores.length) {
          this.error = "Professores não encontrados.";
        }
      } catch (error) {
        this.error = "Erro ao buscar professores.";
        this.professores = [];
      }
    },
  },
  mounted() {
    this.procurarProfessores();
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

#form-professor {
  width: 100%;
  padding: 10px;
}

.professor {
  transition: transform 0.3s ease;
  cursor: pointer;
}

.professor:hover {
  transform: scale(1.01);
  background-color: #f0f8ff;
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

#professores-data {
  overflow-y: auto;
  padding: 10px; /* para evitar que o conteúdo fique colado com a barra de rolagem */
}

/* Estilizar a barra de rolagem, opcional */
#professores-data::-webkit-scrollbar {
  width: 5px;
}
#professores-data::-webkit-scrollbar-thumb {
  background-color: #186fc0;
  border-radius: 10px;
}
</style>
