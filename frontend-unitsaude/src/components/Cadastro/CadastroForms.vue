<template>
  <div class="main-title"></div>
  <div class="card p-4 shadow">
    <div class="top">
      <BackButton class="voltar" />
      <h2 class="tituloCadastro">
        {{ user === "paciente" ? "Cadastro Paciente" : "Cadastro Professor" }}
      </h2>
    </div>

    <!-- Exibição de erros -->
    <div v-if="erro" class="alert alert-danger">
      {{ erro }}
    </div>

    <form @submit.prevent="cadastrar">
      <div class="grid">
        <div class="leftCad p-2">
          <div class="mb-3">
            <label for="cpf" class="form-label">CPF</label>
            <input
              type="text"
              id="cpf"
              class="form-control"
              v-model="cpf"
              @input="formatarCPF"
              placeholder="Digite seu CPF"
              maxlength="14"
            />
          </div>

          <div class="mb-3">
            <label for="nome" class="form-label">Nome</label>
            <input
              type="text"
              id="nome"
              class="form-control"
              v-model="nome"
              placeholder="Digite seu nome"
              required
            />
          </div>
          <div class="mb-3">
            <label for="dataNascimento" class="form-label"
              >Data de Nascimento</label
            >
            <input
              type="date"
              id="dataNascimento"
              class="form-control"
              v-model="dataNascimento"
              required
            />
          </div>

          <div v-if="user === 'professor'">
            <div class="mb-3">
              <label for="area" class="form-label">Área</label>
              <select
                v-model="area"
                id="area"
                class="form-control"
                @change="carregarEspecialidades"
                required
              >
                <option value="" disabled selected>
                  Selecione uma área...
                </option>
                <option
                  v-for="area in areasDisponiveis"
                  :key="area"
                  :value="area"
                >
                  {{ area }}
                </option>
              </select>
            </div>

            <div class="mb-3">
              <label for="especialidades" class="form-label"
                >Especialidades</label
              >
              <div class="multiselect-wrapper">
                <multiselect
                  v-model="especialidadesSelecionadas"
                  id="especialidades"
                  :options="especialidadesDisponiveis"
                  :multiple="true"
                  :close-on-select="false"
                  :clear-on-select="false"
                  :preserve-search="true"
                  placeholder="Selecione as especialidades"
                  label="nome"
                  track-by="nome"
                  :preselect-first="false"
                  required
                >
                  <template
                    slot="selection"
                    slot-scope="{ values, search, isOpen }"
                  >
                    <span
                      class="multiselect__single"
                      v-if="values.length && !isOpen"
                    >
                      {{ values.length }} especialidades selecionadas
                    </span>
                  </template>
                </multiselect>
              </div>
            </div>
          </div>
        </div>
        <div class="RightCad p-2">
          <div v-if="user === 'professor'">
            <div class="mb-3">
              <label for="codigo-profissional" class="form-label"
                >Código profissional</label
              >
              <input
                type="text"
                id="codigo-profissional"
                class="form-control"
                v-model="codigoProfissional"
                placeholder="CRM-PE 123456"
                required
              />
            </div>
          </div>
          <div class="mb-3">
            <label for="telefone" class="form-label">Telefone</label>
            <input
              type="text"
              id="telefone"
              class="form-control"
              v-model="telefone"
              placeholder="(00) 00000-0000"
              required
            />
          </div>

          <div class="mb-3">
            <label for="email" class="form-label">E-mail</label>
            <input
              type="email"
              id="email"
              class="form-control"
              v-model="email"
              placeholder="Digite seu e-mail"
              required
            />
          </div>

          <div class="mb-3">
            <label for="senha" class="form-label">Senha</label>
            <input
              type="password"
              id="senha"
              class="form-control"
              :class="{ 'is-invalid': this.senhaNaoConfere }"
              s
              v-model="senha"
              placeholder="Digite sua senha"
              required
            />
          </div>
          <div class="mb-3" v-if="this.user === 'professor'">
            <label for="confirmarSenha" class="form-label"
              >Confirmar Senha</label
            >
            <input
              type="password"
              id="confirmarSenha"
              class="form-control"
              v-model="confirmarSenha"
              :class="{ 'is-invalid': this.senhaNaoConfere }"
              s
              placeholder="Confirme sua senha"
              required
            />
          </div>
        </div>
      </div>
      <div>
        <div class="mb-3" v-if="this.user != 'professor'">
          <label for="confirmarSenha" class="form-label">Confirmar Senha</label>
          <input
            type="password"
            id="confirmarSenha"
            class="form-control"
            :class="{ 'is-invalid': this.senhaNaoConfere }"
            v-model="confirmarSenha"
            placeholder="Confirme sua senha"
            required
          />
        </div>
        <div v-if="senhaNaoConfere" class="alert alert-danger">
          As senhas não coincidem.
        </div>
      </div>

      <!-- Botão centralizado abaixo -->
      <div class="text-center mt-4">
        <button
          type="submit"
          class="btn btn-primary w-50"
          :disabled="senhaNaoConfere || isLoading"
        >
          <span v-if="!isLoading">Cadastrar</span>
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
</template>

<script>
import api from "@/services/api";
import { useAuthStore } from "@/store/auth";
import Swal from "sweetalert2";
import BackButton from "@/components/btnVoltar.vue";
import Multiselect from "vue-multiselect";
import "vue-multiselect/dist/vue-multiselect.css";

export default {
  props: {
    user: {
      type: String,
      default: "admin",
    },
  },
  components: {
    BackButton,
    Multiselect,
  },
  data() {
    return {
      email: "",
      senha: "",
      confirmarSenha: "",
      nome: "",
      dataNascimento: "",
      telefone: "",
      cpf: "",
      erro: null,
      senhaNaoConfere: false, // Adiciona variável de erro de senha
      codigoProfissional: "",
      area: "",
      especialidade: "",
      isLoading: false,
      areasDisponiveis: [],
      especialidadesDisponiveis: [],
      especialidadesSelecionadas: [],
    };
  },
  watch: {
    confirmarSenha(newValue) {
      this.senhaNaoConfere = newValue !== this.senha;
    },
    cpf(newValue) {
      // Chama a formatação sempre que o CPF muda
      this.formatarCPF();
    },
  },
  methods: {
    formatarCPF() {
      // Remove tudo que não é dígito
      let cpfLimpo = this.cpf.replace(/\D/g, "");

      // Limita a 11 caracteres
      cpfLimpo = cpfLimpo.substring(0, 11);

      // Aplica a formatação do CPF (xxx.xxx.xxx-xx)
      let cpfFormatado = "";
      for (let i = 0; i < cpfLimpo.length; i++) {
        if (i === 3 || i === 6) {
          cpfFormatado += ".";
        } else if (i === 9) {
          cpfFormatado += "-";
        }
        cpfFormatado += cpfLimpo[i];
      }

      this.cpf = cpfFormatado;
    },

    handleHome(auth) {
      if (!auth.token) {
        // Exibe o alerta primeiro
        this.$router.push("/");
      } else if (auth.usuario?.tipoUsuario === "Administrador") {
        this.$router.push("/admin"); // Substitua "/paginaX" pela página que você deseja para o Administrador
      }
    },

    async carregarAreas() {
      try {
        const response = await api.get("/api/Consulta/areas");
        this.areasDisponiveis = response.data.data;
      } catch (error) {
        console.error("Erro ao carregar áreas:", error);
      }
    },

    async carregarEspecialidades() {
      if (!this.area) return;

      try {
        const response = await api.get(
          `/api/Consulta/especialidades/${this.area}`
        );
        this.especialidadesDisponiveis = response.data.data.map((esp) => ({
          nome: esp,
          id: esp.toLowerCase().replace(/\s+/g, "-"),
        }));
      } catch (error) {
        console.error("Erro ao carregar especialidades:", error);
      }
    },

    limparCampos() {
      // Limpa todos os campos de input após o sucesso do cadastro
      this.email = "";
      this.senha = "";
      this.confirmarSenha = "";
      this.nome = "";
      this.dataNascimento = "";
      this.telefone = "";
      this.cpf = "";
      this.codigoProfissional = "";
      this.area = "";
      this.especialidade = "";
      this.especialidadesSelecionadas = "";
    },

    async cadastrarPaciente() {
      const response = await api.post("/api/Paciente/CreatePaciente", {
        cpf: this.cpf,
        nome: this.nome,
        email: this.email,
        senhaHash: this.senha,
        telefone: this.telefone,
        dataNascimento: this.dataNascimento,
      });
    },
    async cadastrarProfessor() {
      // Transforma o array de objetos em array de strings
      const especialidades = this.especialidadesSelecionadas.map(
        (esp) => esp.nome
      );
      const response = await api.post("/api/Professor/CreateProfessor", {
        cpf: this.cpf,
        nome: this.nome,
        email: this.email,
        senhaHash: this.senha,
        telefone: this.telefone,
        dataNascimento: this.dataNascimento,
        area: this.area,
        especialidades: especialidades, // Agora é um array
        codigoProfissional: this.codigoProfissional,
      });
    },
    async cadastrar() {
      const cpfNumeros = this.cpf.replace(/\D/g, "");
      if (cpfNumeros.length !== 11) {
        this.erro = "CPF deve conter 11 dígitos";
        this.isLoading = false;
        return;
      }
      if (this.senha !== this.confirmarSenha) {
        this.erro = "As senhas não coincidem!";
        return;
      }
      this.isLoading = true; // Ativa o spinner
      this.erro = null; // Limpa erros anteriores
      try {
        if (this.user === "paciente") {
          await this.cadastrarPaciente();
        } else if (this.user === "professor") {
          await this.cadastrarProfessor();
        }
        const auth = useAuthStore();

        await Swal.fire({
          icon: "success",
          title: "Cadastro realizado com sucesso!",
          text: "Você será redirecionado em instantes...",
          background: "#ffffff", // fundo branco
          color: "#186fc0", // cor do texto principal (azul do seu tema)
          confirmButtonColor: "#d8bd2c", // botão (amarelo do seu tema)
          timer: 3000, // fecha sozinho em 3 segundos
          timerProgressBar: true, // barra de tempo animada
          showConfirmButton: false, // sem botão de "Ok"
        });

        this.handleHome(auth);
        this.limparCampos(); // Limpa os campos após o sucesso
      } catch (error) {
        if (
          error.response &&
          error.response.data &&
          error.response.data.message
        ) {
          console.error("Erro da API:", error.response.data.message);
          this.erro = error.response.data.message;
        } else {
          console.error("Erro desconhecido:", error);
          this.erro = "Erro inesperado ao tentar realizar cadastro.";
        }
      } finally {
        this.isLoading = false; // Desativa o spinner independente do resultado
      }
    },
  },
  mounted() {
    this.carregarAreas();
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
  max-width: 900px;
  gap: 1rem;
}

.grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr); /* 2 colunas com largura igual */
  gap: 1rem; /* Espaçamento entre os itens */
  align-items: center;
}

.form-group {
  margin-bottom: 1.5rem;
}

.top {
  display: grid;
  grid-template-columns: auto 1fr; /* 2 colunas: uma para a seta e outra para o título */
  align-items: center; /* Alinha os itens verticalmente */
}

.voltar {
  grid-column: 1; /* Coloca a seta na primeira coluna */
}

.tituloCadastro {
  grid-column: 2; /* Coloca o título na segunda coluna */
  text-align: center; /* Centraliza o título na sua célula */
  margin: 0; /* Remove a margem do título */
}

.btn {
  background-color: #d8bd2c;
  border: #d8bd2c;
  transition: 0.3s ease;
}

.btn:hover {
  background-color: #186fc0;
}

.leftCad,
.RightCad {
  width: 100%;
}

.is-invalid {
  border-color: red;
}

:deep(.multiselect__option--highlight) {
  background: #186fc0; /* Azul do seu tema */
  color: white;
}

:deep(.multiselect__option--selected.multiselect__option--highlight) {
  background: #d8bd2c; /* Amarelo do seu tema */
  color: #000;
}

:deep(.multiselect__tag) {
  background: #186fc0; /* Azul do seu tema */
}

:deep(.multiselect__tag-icon:after) {
  color: white;
}

:deep(.multiselect__tag-icon:hover) {
  background: #1357a3; /* Azul mais escuro para hover */
}
</style>
