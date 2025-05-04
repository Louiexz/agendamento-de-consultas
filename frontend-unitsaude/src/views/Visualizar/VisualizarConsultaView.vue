<template>
    <div class="main">
        <VisualizarConsulta/>
    </div>
</template>
  
<script>
import VisualizarConsulta from '@/components/Visualizar/VisualizarConsulta.vue';  
import BackButton from "@/components/btnVoltar.vue";
import Header from "@/components/Header.vue";

export default {
    components: {
        VisualizarConsulta,
        Header,
        BackButton
    },
};
</script>
<style scoped>
.main {
    position: absolute;
    z-index: 99;
    padding: 11.5rem 5%;
    background-color: rgba(0, 0, 0, 0.5);
    height: -webkit-fill-available;
    width: -webkit-fill-available;
}

</style>