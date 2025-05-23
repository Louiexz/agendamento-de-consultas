<template>
  <Header />
  <div
    class="main cadastro-paciente d-flex justify-content-center align-items-center"
  >
    <PacienteForm user="paciente" />
  </div>
</template>

<script>
import api from "@/services/api";
import { useAuthStore } from "@/store/auth";
import BackButton from "@/components/btnVoltar.vue";
import Swal from "sweetalert2";
import Header from "@/components/Header.vue";
import PacienteForm from "@/components/Cadastro/CadastroForms.vue";

export default {
  components: {
    BackButton,
    Header,
    PacienteForm,
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
.cadastro-paciente {
  padding: 4.5rem 15vw;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 30px;
}
</style>
