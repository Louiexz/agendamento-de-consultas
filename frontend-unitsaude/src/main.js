// main.js
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import { useAuthStore } from '@/store/auth'
import { useConsultaStore } from '@/store/consulta'

const app = createApp(App)
const pinia = createPinia()

app.use(pinia) // Registrar pinia primeiro
// Agora os stores podem ser utilizados com segurança
const auth = useAuthStore()
const consulta = useConsultaStore()

consulta.createStore()
auth.carregarToken()

app.use(router)

// Aguardar a montagem da app antes de usar os stores
app.mount('#app')