<template>
  <div class="main">
    <div class="container logo">
      <RouterLink to="/"><img src="../assets/Logo.svg" alt="" /></RouterLink>
    </div>
    <div class="container d-flex justify-content-center align-items-center">
      <div class="card p-4 shadow">
        <h2 class="text-center mb-4">Redefinir Senha</h2>

        <!-- Exibição de erros -->
        <div v-if="erro" class="alert alert-danger">
          {{ erro }}
        </div>

        <!-- Mensagem de sucesso -->
        <div v-if="sucesso" class="alert alert-success text-center">
          Senha redefinida com sucesso!
        </div>

        <form @submit.prevent="redefinirSenha">
          <div class="RightCad p-2" style="flex: 1">
            <div class="mb-3">
              <label for="novaSenha" class="form-label">Nova Senha</label>
              <input
                type="password"
                id="novaSenha"
                class="form-control"
                v-model="novaSenha"
                placeholder="Digite sua nova senha"
                required
              />
            </div>
            <div class="mb-3">
              <label for="confirmarSenha" class="form-label"
                >Confirmar Senha</label
              >
              <input
                type="password"
                id="confirmarSenha"
                class="form-control"
                v-model="confirmarSenha"
                placeholder="Confirme sua nova senha"
                required
              />
            </div>
            <div v-if="senhaNaoConfere" class="alert alert-danger">
              As senhas não coincidem.
            </div>
          </div>

          <!-- Botão centralizado abaixo -->
          <div class="text-center">
            <button
              type="submit"
              class="btn btn-primary w-50"
              :disabled="senhaNaoConfere"
            >
              Redefinir
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import { useRoute, useRouter } from "vue-router";
import api from "@/services/api";
import Swal from "sweetalert2";

export default {
  data() {
    return {
      novaSenha: "",
      confirmarSenha: "",
      erro: null,
      sucesso: false,
      senhaNaoConfere: false,
      token: null,
    };
  },
  watch: {
    confirmarSenha(newValue) {
      this.senhaNaoConfere = newValue !== this.novaSenha;
    },
  },
  created() {
    const route = useRoute();
    this.token = route.query.token; // Captura o token da URL

    // Verifica se o token está presente
    if (!this.token) {
      this.erro = "Token não encontrado na URL!";
      const router = useRouter();
      router.push("/"); // Redireciona para a página inicial se o token não for encontrado
    }
  },
  methods: {
    async redefinirSenha() {
      if (!this.token) {
        this.erro = "Token não encontrado! Verifique a URL.";
        return;
      }

      if (this.novaSenha !== this.confirmarSenha) {
        this.erro = "As senhas não coincidem!";
        return;
      }

      try {
        const response = await api.post("/api/Usuario/redefinir-senha", {
          token: this.token, // Envia o token da URL
          novaSenha: this.novaSenha, // Envia apenas a senha nova para a API
        });

        console.log("Resposta completa da API:", response.data);
        this.sucesso = true; // Exibe a mensagem de sucesso

        await Swal.fire({
          icon: "success",
          title: "Senha redefinida!",
          text: "Você será redirecionado em instantes...",
          background: "#ffffff", // fundo branco
          color: "#186fc0", // cor do texto principal (azul do seu tema)
          confirmButtonColor: "#d8bd2c", // botão (amarelo do seu tema)
          timer: 3000, // fecha sozinho em 3 segundos
          timerProgressBar: true, // barra de tempo animada
          showConfirmButton: false, // sem botão de "Ok"
        });

        this.$router.push("/");
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
          this.erro = "Erro ao tentar redefinir senha.";
        }
      }
    },
  },
};
</script>

<style scoped>
.main {
  background-color: #186fc0;
  padding: 0 15vw;
  padding-bottom: 5%;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 30px;
}
.btn {
  background-color: #d8bd2c;
  border: #d8bd2c;
  transition: 0.3s ease;
}
p {
  color: gray;
}
.btn:hover {
  background-color: #186fc0;
}
.logo {
  align-content: center;
  justify-self: center;
  text-align: center;
}
.logo img {
  width: 15%;
}
</style>
