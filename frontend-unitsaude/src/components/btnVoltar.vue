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
      }  else if (auth.tipoUsuario === "Professor") {
        this.$router.push("/professor");
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
  background-color: none;
}

.bi-arrow-left:hover {
  color: #186fc0;
  transform: translateY(-2px) scale(1.05); /* Leve "pulo" e aumento */
  filter: brightness(1.2) contrast(1.1);  /* Brilho e contraste ajustados */
  filter: brightness(1.2) contrast(1.1) drop-shadow(0 4px 4px rgba(0, 0, 0, 0.4)); 
}


</style>
