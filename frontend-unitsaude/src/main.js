// main.js
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'
import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'

const app = createApp(App)

const pinia = createPinia()
pinia.use(piniaPluginPersistedstate)

// ✅ Primeiro registra o Pinia
app.use(pinia)
app.use(router)

// ✅ Só agora é seguro usar os stores
import { useAuthStore } from '@/store/auth'
import { useConsultaStore } from '@/store/consulta'
import { useUsuarioStore } from '@/store/usuario'
const auth = useAuthStore()
const consulta = useConsultaStore()
const usuario = useUsuarioStore()

consulta.createStore()
usuario.createStore()
auth.carregarToken()

// ✅ Agora pode montar
app.mount('#app')
