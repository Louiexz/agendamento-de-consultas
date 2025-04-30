<template>
    <i class="bi bi-arrow-left" @click="handleClick"></i>
</template>


<script>
import { useAuthStore } from "@/store/auth"; // Certifique-se de importar a store

export default {
  name: "BackButton",
  methods: {
    handleClick() {
      const auth = useAuthStore();

      if (!auth.token) {
        // Se o usuário não estiver logado, redireciona para a página inicial
        this.$router.push("/");
      } else if (auth.tipoUsuario === "Administrador") {
        // Se o usuário for um administrador, redireciona para uma página específica
        this.$router.push("/admin"); // Substitua '/paginaX' pela página do administrador
      }
    },
  },
};
</script>

<style scoped>

.bi-arrow-left {
  color: #d8bd2c;
  font-size: 40px;
  cursor: pointer;
  transition: color 0.3s ease;
}

.bi-arrow-left:hover {
  color: #186fc0;
}

</style>
