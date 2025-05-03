<template>
  <div class="main min-vh-100">
    <div class="container logoL">
      <img src="../../assets/Logo.svg" alt="" />
    </div>
    <div
      class="container d-flex justify-content-center align-items-center "
    >
      <div class="card p-4 shadow" style="width: 100%; max-width: 400px">
        <h2 class="text-center mb-4">Login</h2>

        <!-- Exibição de erros -->
        <div v-if="erro" class="alert alert-danger">
          {{ erro }}
        </div>

        <form @submit.prevent="fazerLogin">
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
            <span>
              <RouterLink to="/recuperarSenha" class="link recuperar">
                Esqueceu sua senha ?</RouterLink
              ></span
            >
          </div>
          <div class="text-center">
            <!-- reCAPTCHA v2 Checkbox -->
            <div
              v-if="checkCaptcha"
              class="g-recaptcha"
              data-sitekey="6Lc6yigrAAAAAG3SV8WLTI-Aj9KZ5YonZh_7dpUr"
              data-callback="onCaptchaVerified"
              data-expired-callback="onCaptchaExpired"
            ></div>
            <button
              :disabled="!captchaVerified"
              type="submit"
              class="btn btn-primary w-50"
            >
              Entrar
            </button>
          </div>
        </form>
        <span>
          <RouterLink to="/registrar" class="link">
            Registre-se
          </RouterLink>
        </span>
      </div>
    </div>
  </div>
</template>

<script>
import api from "@/services/api";
import { useAuthStore } from "@/store/auth";

export default {
  data() {
    return {
      email: "",
      senha: "",
      erro: null,
      checkCaptcha: false,
      captchaVerified: true,
      userAttempts: 0,
    };
  },
  methods: {
    onCaptchaVerified() {
      this.captchaVerified = true;
    },
    onCaptchaExpired() {
      this.captchaVerified = false;
      grecaptcha.reset();
    },
    loadRecaptchaScript() {
      let actualScript = document.querySelector("#recaptcha-script");
      if (actualScript) {
        grecaptcha.reset();
      } else {
        //document.head.removeChild(actualScript);
        const script = document.createElement("script");
        script.id = "recaptcha-script";
        script.src = "https://www.google.com/recaptcha/api.js";
        script.async = true;
        script.defer = true;
        document.head.appendChild(script);
      }
    },
    async fazerLogin() {
      try {
        if (this.checkCaptcha) {
          if (!this.captchaVerified) {
            alert("Por favor, confirme que você não é um robô.");
            return;
          }
          const captchaToken = grecaptcha.getResponse();

          const tokenResponse = await api.post("/api/Usuario/CheckCaptcha", {
            captchaToken: captchaToken,
          });

          if (tokenResponse.status == false)
            return alert("Verificação de captcha falhou.");
        }

        const response = await api.post("/api/Usuario/Login", {
          credential: this.email,
          password: this.senha,
        });

        // Se a resposta for bem-sucedida, pega o token
        const token = response.data.token;
        const tipoUsuario = response.data.usuario.tipoUsuario;
        const nomeUsuario = response.data.usuario.nome;

        const auth = useAuthStore();
        auth.setToken(token);
        auth.setNomeUsuario(nomeUsuario);
        auth.setTipoUsuario(tipoUsuario);
        
        if (tipoUsuario === "Administrador") {
          this.$router.push("/admin");
        } else if (tipoUsuario === "Professor") {
          this.$router.push("/professor");
        } else if (tipoUsuario === "Paciente") {
          this.$router.push("/paciente");
        } else {
          this.erro = "Tipo de usuário desconhecido.";
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
          this.erro = "Erro inesperado ao tentar logar.";
        }
        this.userAttempts += 1;

        if (this.userAttempts >= 3) {
          if (this.userAttempts === 3) this.checkCaptcha = true;
          this.captchaVerified = false;
          this.loadRecaptchaScript();
        }
      }
    },
  },
  mounted() {
    // Adicione isso para tornar os callbacks acessíveis globalmente
    window.onCaptchaVerified = () => this.onCaptchaVerified();
    window.onCaptchaExpired = () => this.onCaptchaExpired();
  },
};
</script>

<style scoped>
.main {
  background-color: #186fc0;
  padding: 0 15vw;
  display: grid;
  grid-template-columns: repeat(2, 1fr); /* 2 colunas com largura igual */

}
.logoL {
  align-content: center;
  justify-self: center;
  text-align: center;
}

.logoL img {
  width: 80%;
}
.btn {
  background-color: #d8bd2c;
  border: #d8bd2c;
  transition: 0.3s ease;
}

.btn:hover {
  background-color: #186fc0;
}

.link {
  text-align: center;
  text-decoration: none;
  transition: 0.3s ease;
  color: black;
}

.recuperar {
  font-size: 14px;
  color: gray;
  padding-left: 5px;
}
.link:hover {
  color: #186fc0;
}

span {
  text-align: center;
  padding-top: 7px;
}

@media (max-width: 690px) {
  .main {
    display: grid;
    grid-template-columns: 1fr; /* 2 colunas com largura igual */
    justify-content: center;
    align-content: center;
   gap: 1rem;
  }


  .logoL img {
    width:230px;
  }


}
</style>
