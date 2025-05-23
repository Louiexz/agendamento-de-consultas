<template>
  <div class="main d-flex justify-content-center align-items-center">
    <div class="container logoL">
      <RouterLink to="/"><img src="../../assets/Logo.svg" alt="" /></RouterLink>
    </div>
    <PacienteForm view = "registrar-se" user="paciente"/>
  </div>
</template>

<script>
import api from "@/services/api";
import { useAuthStore } from "@/store/auth";
import BackButton from "@/components/btnVoltar.vue";
import PacienteForm from "@/components/Cadastro/CadastroForms.vue";
import Swal from "sweetalert2";

export default {
  components: {
    BackButton,
    PacienteForm, // Registre o componente com a inicial maiúscula
    // BtnVoltar,
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
          this.$router.push("/admin"); // Substitua "/paginaX" pela página que você deseja para o Administrador
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

.top {
  position: relative;
  display: grid;
  grid-template-columns: auto 1fr;
}

.btn {
  background-color: #d8bd2c;
  border: #d8bd2c;
  transition: 0.3s ease;
}

.btn:hover {
  background-color: #186fc0;
}
.logoL {
  align-content: center;
  justify-self: center;
  text-align: center;
}
.logoL img {
  width: 25%;
}

@media (max-width: 690px) {

  .logoL img {
    width: 230px;
  }
}
</style>
