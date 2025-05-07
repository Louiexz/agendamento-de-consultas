<template>
  <Header />
  <div class="d-flex justify-content-center align-items-center min-vh-100">
    <div class="form">
      <div class="title mb-4">
        <BackButton />
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
        <!-- Spinner durante busca -->
        <div v-if="isLoading" class="text-center my-3">
          <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Carregando...</span>
          </div>
        </div>

        <div
          v-for="(professorInfo, idx) in professores"
          :key="professorInfo.id || idx"
          class="professor card p-2 align-items-start shadow-sm"
        >
          <RouterLink
            @click="verPerfilProfessor(professorInfo)"
            to="/perfilProfessor"
            class="professor-info"
          >
            {{ professorInfo.nome }} {{ professorInfo.especialidade }}
          </RouterLink>
        </div>
      </div>
      <!-- Spinner durante carregamento inicial -->
      <div
        v-if="isLoading && professores.length === 0"
        class="text-center my-3"
      >
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Carregando...</span>
        </div>
        <p>Carregando professores...</p>
      </div>

      <!-- Mensagem quando não há resultados -->
      <div
        v-if="!professores.length && !isLoading && !error"
        class="text-center my-3"
      >
        Nenhum professor encontrado
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
  name: "VisualizarProfessores",
  data() {
    return {
      professor: "",
      professores: [],
      error: "",
      isLoading: false, // Adicione esta linha
    };
  },
  components: {
    Header,
    BackButton,
  },
  methods: {
    verPerfilProfessor(usuarioSelecionado) {
      const usuarioStore = useUsuarioStore();
      
      usuarioStore.setUsuario(usuarioSelecionado);
    },
    capitalizar(str) {
      if (!str) return "";
      return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
    },
    async procurarProfessores() {
      this.isLoading = true; // Ativa o spinner
      this.error = "";
      try {
        const isEspecialidade = this.professor.includes(" ");

        // Se campo está vazio, lista todos
        if (this.professor === "") {
          const res = await api.get("/api/Professor/ListarTodos");
          this.professores = res.data.data || [];
          this.error = ""; // limpa erro se campo está vazio
          return;
        }

        // Busca com filtro
        const res = await api.get("/api/Professor/ListarComFiltro", {
          params: {
            nome: isEspecialidade ? "" : this.capitalizar(this.professor),
            especialidade: isEspecialidade
              ? this.capitalizar(this.professor)
              : "",
          },
        });

        this.professores = res.data.data || [];

        if (this.professores.length) {
          this.error = ""; // limpa erro se encontrar resultados
        } else {
          this.error = "Professores não encontrados.";
        }
      } catch (error) {
        this.error = "Erro ao buscar professores.";
        this.professores = [];
      } finally {
        this.isLoading = false; // Desativa o spinner independente do resultado
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
  display: grid;
  grid-template-columns: auto 1fr; /* 2 colunas: uma para a seta e outra para o título */
  align-items: center; /* Alinha os itens verticalmente */
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

.professor-info {
  width: 100%;
  height: 100%;
  text-decoration: none;
  color: black;
}

.professor:hover {
  transform: scale(1.01);
  background-color: #f0f8ff;
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
