<template>
    <div v-if="isConsultaAtiva">
      <VisualizarConsultaView :consulta="consulta"/>
      <button id="fechar-consulta" @click="fecharConsulta">X</button>
    </div>
	<div class="consulta">
	  <span><b>{{ this.consulta.data }} às {{ this.consulta.horario }}</b></span>
	  <span><i class="bi bi-clipboard2-pulse-fill"></i> {{ this.consulta.area }}</span>
	  <span><i class="bi bi-geo-alt-fill"></i> Centro Universitário Tiradentes - UNIT PE</span>
	  <button class="consulta-detalhes" @click="verDetalhes(this.consulta)">Ver detalhes</button>
	 </div>
</template>

<script>
import VisualizarConsultaView from '@/views/Visualizar/VisualizarConsultaView.vue';
import { useConsultaStore } from "@/store/consulta";

export default {
	name: "ConsultaView",
	props: {
		consulta: {
			type: Object,
			required: true
		}
	},
	data() {
		return {
      isConsultaAtiva: false,
      scrollPosition: 0
		}
	},
	components: {
    	VisualizarConsultaView
	},
	methods: {
			fecharConsulta(){
    		this.isConsultaAtiva = false;
    		window.scrollTo({ top: this.scrollPosition, behavior: 'smooth' });
			},
	    verDetalhes(consultaSelecionada) {
	      const consultaStore = useConsultaStore();

	      consultaStore.setConsulta(consultaSelecionada);

	      this.isConsultaAtiva = true;

	      this.scrollPosition = window.scrollY;

	      window.scrollTo({ top: 0, behavior: 'smooth' });
	    },
	}	
}
</script>
.consulta-detalhes {
  text-align: center;
  min-width: 100%
}
<style>
.consulta {
  display: flex;
  flex-direction: column;
  box-shadow: 1px 1px 1px 1px;
  align-items: flex-start;
  padding: 20px;
  gap: 20px;
  min-width: 100%
}
#fechar-consulta {
  z-index: 99;
  color: red;
  position: absolute;
  right: 7%;
  top: 13rem
}
</style>