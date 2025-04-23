<template>
  <div>
    <!-- Mostrar o botão de logout apenas se o token estiver presente -->
    <button v-if="auth.token" @click="logout" class="btn btn-danger">Logout</button>
  </div>
</template>

<script>
import { useAuthStore } from '@/store/auth'
import { useRouter } from 'vue-router'
import { onMounted, computed } from 'vue'

export default {
  setup() {
    const auth = useAuthStore()
    const router = useRouter()

    // Função de logout
    const logout = () => {
      auth.logout()  // Limpa o estado de autenticação
      router.push('/') // Redireciona para a página de login
    }

    // Usando `onMounted` para garantir que o token seja carregado ao montar o componente
    onMounted(() => {
      auth.carregarToken() // Carrega o token do localStorage quando o componente for montado
    })

    // Usando `computed` para garantir que o botão só apareça se o token estiver presente
    const isLoggedIn = computed(() => auth.token !== null)

    return {
      auth,
      logout,
      isLoggedIn, // Expor a variável computada para ser usada no template
    }
  },
}
</script>

<style scoped>
.btn {
  background-color: #d8bd2c;
  border: #d8bd2c;
}
</style>
