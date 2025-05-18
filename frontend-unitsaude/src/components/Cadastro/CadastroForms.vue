<template>
  <div class="main-title"></div>
  <div class="card p-4 shadow">
    <div class="top">
      <BackButton class="voltar" />
      <h2 class="tituloCadastro">
        {{ user === "paciente" ? "Cadastro Paciente" : "Cadastro Professor" }}
      </h2>
    </div>

    <!-- Exibição de erros da API ou validação geral -->
    <div v-if="erroApi" class="alert alert-danger">
      {{ erroApi }}
    </div>

    <form @submit.prevent="cadastrar">
      <div class="grid">
        <div class="leftCad p-2">
          <!-- CPF com Máscara e Validação -->
          <div class="mb-3 position-relative">
            <label for="cpf" class="form-label">CPF</label>
            <input
              type="text"
              id="cpf"
              class="form-control"
              :class="{
                'is-invalid': erroCPF,
                'is-valid': cpf && !erroCPF && isCpfPotencialmenteValido,
              }"
              v-model="cpf"
              v-maska
              data-maska="###.###.###-##"
              @blur="validarCPF"
              placeholder="000.000.000-00"
            />
            <div v-if="erroCPF" class="invalid-feedback d-block">
              {{ erroCPF }}
            </div>
            <div v-if="cpf && !erroCPF && isCpfPotencialmenteValido" class="valid-feedback d-block">
              CPF válido
            </div>
          </div>

          <!-- Nome -->
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

          <!-- Data de Nascimento -->
          <div class="mb-3">
            <label for="dataNascimento" class="form-label">Data de Nascimento</label>
            <input
              type="date"
              id="dataNascimento"
              class="form-control"
              v-model="dataNascimento"
              required
            />
          </div>

          <!-- Campos do Professor -->
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
                  v-for="areaOp in areasDisponiveis"
                  :key="areaOp"
                  :value="areaOp"
                >
                  {{ areaOp }}
                </option>
              </select>
            </div>
            <div class="mb-3">
              <label for="especialidades" class="form-label">Especialidades</label>
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
                  <template slot="selection" slot-scope="{ values, search, isOpen }">
                    <span class="multiselect__single" v-if="values.length && !isOpen">
                      {{ values.length }} especialidades selecionadas
                    </span>
                  </template>
                </multiselect>
              </div>
            </div>
          </div>
        </div>

        <div class="RightCad p-2">
          <!-- Código Profissional -->
          <div v-if="user === 'professor'">
            <div class="mb-3">
              <label for="codigo-profissional" class="form-label">Código profissional</label>
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

          <!-- Telefone com Máscara -->
          <div class="mb-3">
            <label for="telefone" class="form-label">Telefone</label>
            <input
              type="tel"
              id="telefone"
              class="form-control"
              v-model="telefone"
              v-maska
              :data-maska="['(##) ####-####', '(##) #####-####']"
              placeholder="(00) 00000-0000"
            />
          </div>

          <!-- E-mail -->
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

          <!-- Senha -->
          <div class="mb-3">
            <label for="senha" class="form-label">Senha</label>
            <input
              type="password"
              id="senha"
              class="form-control"
              :class="{ 'is-invalid': senhaNaoConfere }"
              v-model="senha"
              placeholder="Digite sua senha"
              required
            />
          </div>

          <!-- Confirmar Senha (Professor) -->
          <div class="mb-3" v-if="user === 'professor'">
            <label for="confirmarSenhaProf" class="form-label">Confirmar Senha</label>
            <input
              type="password"
              id="confirmarSenhaProf"
              class="form-control"
              v-model="confirmarSenha"
              :class="{ 'is-invalid': senhaNaoConfere }"
              placeholder="Confirme sua senha"
              required
            />
          </div>
        </div>
      </div>

      <!-- Confirmar Senha (Paciente/Outros) -->
      <div v-if="user !== 'professor'">
        <div class="mb-3">
          <label for="confirmarSenhaPac" class="form-label">Confirmar Senha</label>
          <input
            type="password"
            id="confirmarSenhaPac"
            class="form-control"
            v-model="confirmarSenha"
            :class="{ 'is-invalid': senhaNaoConfere }"
            placeholder="Confirme sua senha"
            required
          />
        </div>
      </div>

      <div v-if="senhaNaoConfere && !erroApi" class="alert alert-danger">
        As senhas não coincidem.
      </div>

      <!-- Botão centralizado abaixo -->
      <div class="text-center mt-4">
        <button
          type="submit"
          class="btn btn-primary w-50"
          :disabled="senhaNaoConfere || isLoading"
        >
          <span v-if="!isLoading">Cadastrar</span>
          <span v-else class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span>
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

// Importar a função de validação da biblioteca correta
// Se você instalou 'validation-br':
import { isCPF } from 'validation-br';
// Se você instalou 'cpf-cnpj-validator', o import que você tinha estava correto para validação:
// import { cpf as cpfValidator } from "cpf-cnpj-validator"; // Renomeando para evitar conflito com data.cpf

// Maska.js não precisa ser importada aqui se registrada globalmente em main.js

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
      telefone: "", // Maska irá formatar este valor
      cpf: "",      // Maska irá formatar este valor
      erroCPF: null, // Erro específico para CPF
      erroApi: null, // Para erros gerais da API ou validações não cobertas por campos específicos
      senhaNaoConfere: false,
      codigoProfissional: "",
      area: "",
      especialidade: "",
      isLoading: false,
      areasDisponiveis: [],
      especialidadesDisponiveis: [],
      especialidadesSelecionadas: [],
    };
  },
  computed: {
    // Para o feedback visual 'is-valid' do CPF
    isCpfPotencialmenteValido() {
      const cpfLimpo = this.cpf ? this.cpf.replace(/\D/g, '') : '';
      return cpfLimpo.length === 11;
    }
  },
  watch: {
    confirmarSenha(newValue) {
      this.senhaNaoConfere = newValue !== this.senha;
    },
    cpf(newValue, oldValue) {
      // Validar no blur é geralmente melhor para UX, mas se quiser no watch:
      // if (newValue && newValue.replace(/\D/g, '').length === 11) {
      //   this.validarCPF();
      // } else if (newValue && newValue.replace(/\D/g, '').length > 0) {
      //   this.erroCPF = null; // Limpa erro se não estiver completo para validação ainda
      // }
    },
  },
  methods: {
    // formatarCPF() { // REMOVIDO - Maska.js cuida disso
    // },
    validarCPF() {
      const cpfLimpo = this.cpf ? this.cpf.replace(/\D/g, '') : '';
      if (!cpfLimpo) {
         this.erroCPF = "O CPF é obrigatório."; // Pode ser redundante se o campo for 'required'
        this.erroCPF = null; // Ou limpa se vazio e não obrigatório explicitamente aqui
        return true; // Considerar válido se não obrigatório e vazio, ou deixar a validação de 'required' HTML
      }
      // Use a função importada correta:
      if (!isCPF(cpfLimpo)) { // Usando isCPF de validation-br
      // if (!cpfValidator.isValid(cpfLimpo)) { // Se estivesse usando cpf-cnpj-validator
        this.erroCPF = "CPF inválido";
        return false;
      }
      this.erroCPF = null;
      return true;
    },
    handleHome(auth) {
      if (!auth.token) {
        this.$router.push("/");
      } else if (auth.usuario?.tipoUsuario === "Administrador") {
        this.$router.push("/admin");
      }
    },
    async carregarAreas() {
      try {
        const response = await api.get("/api/Consulta/areas");
        this.areasDisponiveis = response.data.data;
      } catch (error) {
        console.error("Erro ao carregar áreas:", error);
        this.erroApi = "Falha ao carregar áreas.";
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
        this.erroApi = "Falha ao carregar especialidades.";
      }
    },
    limparCampos() {
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
      this.especialidadesSelecionadas = [];
      this.erroCPF = null;
      this.erroApi = null;
      this.senhaNaoConfere = false;
      this.isLoading = false;
    },
    getValoresLimpos() {
      return {
        cpf: this.cpf ? this.cpf.replace(/\D/g, '') : '',
        telefone: this.telefone ? this.telefone.replace(/\D/g, '') : '',
      };
    },
    async cadastrarPaciente() {
      const valoresLimpos = this.getValoresLimpos();
      return api.post("/api/Paciente/CreatePaciente", {
        cpf: valoresLimpos.cpf,
        nome: this.nome,
        email: this.email,
        senhaHash: this.senha,
        telefone: valoresLimpos.telefone,
        dataNascimento: this.dataNascimento,
      });
    },
    async cadastrarProfessor() {
      const valoresLimpos = this.getValoresLimpos();
      const especialidades = this.especialidadesSelecionadas.map(
        (esp) => esp.nome
      );
      return api.post("/api/Professor/CreateProfessor", {
        cpf: valoresLimpos.cpf,
        nome: this.nome,
        email: this.email,
        senhaHash: this.senha,
        telefone: valoresLimpos.telefone,
        dataNascimento: this.dataNascimento,
        area: this.area,
        especialidades: especialidades,
        codigoProfissional: this.codigoProfissional,
      });
    },
    async cadastrar() {
      this.erroApi = null; // Limpa erro geral da API
      this.senhaNaoConfere = false; // Reseta estado de erro de senha

      // 1. Validar CPF
      if (!this.validarCPF()) {
        // erroCPF já foi setado e será exibido
        return;
      }

      // 2. Validar Senhas
      if (this.senha !== this.confirmarSenha) {
        this.senhaNaoConfere = true;
        // this.erroApi = "As senhas não coincidem!"; // O v-if="senhaNaoConfere" já exibe
        return;
      }

      this.isLoading = true;
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
          background: "#ffffff",
          color: "#186fc0",
          confirmButtonColor: "#d8bd2c",
          timer: 3000,
          timerProgressBar: true,
          showConfirmButton: false,
        });

        this.handleHome(auth);
        this.limparCampos();
      } catch (error) {
        if (
          error.response &&
          error.response.data &&
          error.response.data.message
        ) {
          console.error("Erro da API:", error.response.data.message);
          this.erroApi = error.response.data.message;
        } else {
          console.error("Erro desconhecido:", error);
          this.erroApi = "Erro inesperado ao tentar realizar cadastro.";
        }
      } finally {
        this.isLoading = false;
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
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
  align-items: flex-start;
}
.top {
  display: grid;
  grid-template-columns: auto 1fr;
  align-items: center;
}
.voltar {
  grid-column: 1;
}
.tituloCadastro {
  grid-column: 2;
  text-align: center;
  margin: 0;
}
.btn {
  background-color: #d8bd2c;
  border-color: #d8bd2c;
  transition: 0.3s ease;
}
.btn:hover {
  background-color: #186fc0;
  border-color: #186fc0;
}
.leftCad,
.RightCad {
  width: 100%;
  display: flex;
  flex-direction: column;
}
.mb-3 {
  margin-bottom: 1rem !important;
}
.form-control.is-invalid { /* Classe específica para inputs de formulário */
  border-color: #dc3545 !important;
}
.form-control.is-valid { /* Classe específica para inputs de formulário */
  border-color: #198754 !important;
}
.invalid-feedback.d-block, .valid-feedback.d-block {
    display: block !important;
}
:deep(.multiselect__option--highlight) {
  background: #186fc0;
  color: white;
}
:deep(.multiselect__option--selected.multiselect__option--highlight) {
  background: #d8bd2c;
  color: #000;
}
:deep(.multiselect__tag) {
  background: #186fc0;
}
:deep(.multiselect__tag-icon:after) {
  color: white;
}
:deep(.multiselect__tag-icon:hover) {
  background: #1357a3;
}
</style>