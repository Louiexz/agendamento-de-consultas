// main.js
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import { useAuthStore } from '@/store/auth'

const app = createApp(App)
const pinia = createPinia()

// Use o Pinia antes de acessar o store
app.use(pinia)

// Agora você pode acessar o store de autenticação
const auth = useAuthStore()

// Carregar o token de localStorage
auth.carregarToken()

app.use(router)
app.mount('#app')
