<template>
  <div class="main">
    <div class="container logo">
      <RouterLink to="/"><img src="../assets/Logo.svg" alt="" /></RouterLink>
    </div>
    <div class="container d-flex justify-content-center align-items-center">
      <div class="card p-4 shadow">
        <div class="top">
          <BackButton />
          <h2 class="text-center mb-4">Recuperar Senha</h2>
        </div>
        <!-- Exibição de erros -->
        <div v-if="erro" class="alert alert-danger">
          {{ erro }}
        </div>

        <!-- Mensagem de sucesso -->
        <!-- Mensagem de sucesso com transição -->
        <transition
          name="fade"
          @before-enter="beforeEnter"
          @enter="enter"
          @leave="leave"
        >
          <div v-if="emailEnviado" class="alert alert-success text-center">
            Verifique seu e-mail!
          </div>
        </transition>

        <form @submit.prevent="recuperarSenha">
          <div class="RightCad p-2" style="flex: 1">
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
          </div>

          <!-- Botão centralizado abaixo -->
          <div class="text-center">
            <button type="submit" class="btn btn-primary w-50">Enviar</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import api from "@/services/api";
import BackButton from "@/components/btnVoltar.vue";
export default {
  components: {
    BackButton,
  },
  data() {
    return {
      email: "",
      erro: null,
      emailEnviado: false,
    };
  },
  methods: {
    async recuperarSenha() {
      try {
        const response = await api.post("/api/Usuario/recuperar-senha", {
          email: this.email,
        });

        console.log("Resposta completa da API:", response.data);
        this.emailEnviado = true; // exibe a mensagem de sucesso
        setTimeout(() => {
          this.emailEnviado = false;
        }, 5000);
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
          this.erro = "Erro inesperado ao tentar enviar e-mail.";
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

.top {
  position: relative;
  display: grid;
  grid-template-columns: auto 1fr;        
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
/* Adicionando a transição de fade */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s ease;
}

.fade-enter, .fade-leave-to /* .fade-leave-active em versões <2.1.8 */ {
  opacity: 0;
}

.card {
  width: 40%;
}

</style>
