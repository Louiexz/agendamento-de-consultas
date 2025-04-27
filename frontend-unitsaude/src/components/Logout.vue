<template>
  <div>
    <i class="bi bi-power" v-if="isLoggedIn" @click="logout"></i>
  </div>
</template>

<script>
import { computed } from 'vue'
import { useAuthStore } from '@/store/auth'
import { useRouter } from 'vue-router'

export default {
  setup() {
    const auth = useAuthStore()
    const router = useRouter()

    const logout = async () => {
      await auth.logout()
      router.push('/login') 
    }

    return {
      logout,
      isLoggedIn: computed(() => auth.token !== null),
    }
  },
}

</script>

<style scoped>
.bi-power {

  color: #fcfcfc;
  font-size: 30px;
  cursor: pointer;
  transition: 0.3s ease;
}
.bi-power:hover {
  color: #d8bd2c;
}

</style>