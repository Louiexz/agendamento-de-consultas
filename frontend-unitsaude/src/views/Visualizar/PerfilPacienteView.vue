<template>
    <Header/>
    <PerfilPaciente/>
</template>

<script>
import PerfilPaciente from '@/components/Visualizar/PerfilPaciente';
import Header from "@/components/Header.vue";

export default {
    components: {
        Header,
        PerfilPaciente
    }
}
</script>