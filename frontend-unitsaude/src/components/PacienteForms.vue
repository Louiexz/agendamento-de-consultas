<template>

    <div class="card p-4 shadow">
      <div class="top">
        <h2 class="text-center mb-4">Cadastro Paciente</h2>
      </div>
      <!-- Exibição de erros -->
      <div v-if="erro" class="alert alert-danger">
        {{ erro }}
      </div>

      <form @submit.prevent="cadastrarPaciente">
        <div class="grid">
          <div class="leftCad p-2">
            <div class="mb-3">
              <label for="cpf" class="form-label">CPF</label>
              <input
                type="text"
                id="cpf"
                class="form-control"
                v-model="cpf"
                placeholder="Digite seu CPF"
                required
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
          </div>
          <div class="RightCad p-2">
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
                v-model="senha"
                placeholder="Digite sua senha"
                required
              />
            </div>
          </div>
        </div>
        <div class="mb-3">
          <label for="confirmarSenha" class="form-label">Confirmar Senha</label>
          <input
            type="password"
            id="confirmarSenha"
            class="form-control"
            v-model="confirmarSenha"
            placeholder="Confirme sua senha"
            required
          />
        </div>
        <div v-if="senhaNaoConfere" class="alert alert-danger">
          As senhas não coincidem.
        </div>

        <!-- Botão centralizado abaixo -->
        <div class="text-center mt-4">
          <button
            type="submit"
            class="btn btn-primary w-50"
            :disabled="senhaNaoConfere"
          >
            Cadastrar
          </button>
        </div>
      </form>
    </div>

</template>

<script>
import api from "@/services/api";
import { useAuthStore } from "@/store/auth";
import Swal from "sweetalert2";

export default {
  components: {},
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
    };
  },
  watch: {
    confirmarSenha(newValue) {
      this.senhaNaoConfere = newValue !== this.senha;
    },
  },
  methods: {
    async cadastrarPaciente() {
      if (this.senha !== this.confirmarSenha) {
        this.erro = "As senhas não coincidem!";
        return;
      }

      try {
        const response = await api.post("/api/Paciente/CreatePaciente", {
          cpf: this.cpf,
          nome: this.nome,
          email: this.email,
          senhaHash: this.senha,
          telefone: this.telefone,
          dataNascimento: this.dataNascimento,
        });

        console.log("Resposta completa da API:", response.data);

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

        if (!auth.token) {
          // Exibe o alerta primeiro
          this.$router.push("/");
        } else if (auth.usuario?.tipoUsuario === "Administrador") {
          this.$router.push("/paginaX"); // Substitua "/paginaX" pela página que você deseja para o Administrador
        }
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
      }
    },
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
}


.form-group {
  margin-bottom: 1.5rem;
}

.main {
  padding: 0 15vw;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 30px;
}

.top {
  text-align: center;
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

</style>
