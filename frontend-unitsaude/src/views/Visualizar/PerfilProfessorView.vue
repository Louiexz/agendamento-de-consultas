<template>
    <Header/>
    <Perfil :isProfessor="true" />
</template>

<script>
import Perfil from '@/components/Visualizar/Perfil';
import Header from "@/components/Header.vue";

export default {
    components: {
        Header,
        Perfil
    }
}
</script>