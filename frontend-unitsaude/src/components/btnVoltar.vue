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
  font-size: 35px;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); /* sombra leve inicial */
  border-radius: 100%;
  padding: 8px;
  background-color: none;
}

.bi-arrow-left:hover {
  color: #186fc0;
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2); /* sombra mais forte no hover */
  transform: translateY(-2px) scale(1.05);  /* leve "pulo" e aumento */
}


</style>
