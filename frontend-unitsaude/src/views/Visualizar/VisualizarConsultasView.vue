<template>
  <VisualizarConsultas :area="area" />
</template>

<script>
import { useRoute } from 'vue-router';
import VisualizarConsultas from '@/components/Visualizar/VisualizarConsultas.vue';
import { defineComponent, computed } from 'vue';

export default defineComponent({
  components: {
    VisualizarConsultas
  },
  setup() {
    const route = useRoute();

    const area = computed(() => route.params.area);
    return {
      area
    };
  }
});
</script>
