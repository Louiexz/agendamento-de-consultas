<template>
  <Header />
  <div class="main cadastro-professor d-flex justify-content-center align-items-center">
    <CadastroForm user="professor" />
  </div>
</template>

<script>
import api from "@/services/api";
import { useAuthStore } from "@/store/auth";
import BackButton from "@/components/btnVoltar.vue";
import Swal from "sweetalert2";
import Header from "@/components/Header.vue";
import CadastroForm from "@/components/Cadastro/CadastroForms.vue";

export default {
  components: {
    BackButton,
    Header,
    CadastroForm,
  },
};
</script>
<style scoped>
.cadastro-professor {
  padding: 3rem 15vw;
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  gap: 30px;
}
</style>