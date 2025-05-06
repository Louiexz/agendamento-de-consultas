// main.js
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
import { useAuthStore } from '@/store/auth'
import { useConsultaStore } from '@/store/consulta'
import { useUsuarioStore } from '@/store/usuario'

const app = createApp(App)

const pinia = createPinia()
pinia.use(piniaPluginPersistedstate) // <-- Adicione isto antes de app.use(pinia)

app.use(pinia) // Registrar pinia primeiro
// Agora os stores podem ser utilizados com segurança
const auth = useAuthStore()
const consulta = useConsultaStore()
const usuario = useUsuarioStore()

consulta.createStore()
usuario.createStore()
auth.carregarToken()

app.use(router)

// Aguardar a montagem da app antes de usar os stores
app.mount('#app')