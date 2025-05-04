<template>
    <div>
        <div v-if="consulta" class="consulta">
            <span>Professor(a): {{ consulta.nomeProfessor }}</span>
            <span>Paciente: {{ consulta.nomePaciente }}</span>
            <span>Área: {{ consulta.area }}</span>
            <span>Especialidade: {{ consulta.especialidade }}</span>
            <span>Status: {{ consulta.status }}</span>
            <span>Data e hora: <b>{{ consulta.data }} às {{ consulta.horario }}</b></span>
            <div class="buttons">
                <button class="btn" disabled>Alterar status</button>
                <button class="btn" disabled>Reagendar consulta</button>
                <!--<button class="btn" disabled>Visualizar prontuário</button>-->
            </div>
        </div>
        <span v-else class="aviso">Consulta não encontrada.</span>
    </div>
</template>

<script>
import api from "@/services/api";
import { useConsultaStore } from "@/store/consulta";

export default {
    data() {
        return {
            erro: "",
            consulta: []
        };
    },
    methods: {
        carregaConsulta() {
            const consultaStore = useConsultaStore();

            this.consulta = consultaStore.getConsulta();
        }
    },
    mounted() {
        this.carregaConsulta();

        if (Object.keys(this.consulta).length === 0) {
            console.log(this.consulta);
            this.$router.go(-1);
        }
    }
};
</script>

<style scoped>
.aviso {
    text-align: center;
    padding: 10px
}
.buttons {
    min-width: 100%;
    display: flex;
    justify-content: center;
    gap: 10px
}
.consulta {
    background-color: white;
    display: flex;
    flex-direction: column;
    box-shadow: 1px 1px 1px 1px;
    align-items: flex-start;
    padding: 40px;
    gap: 20px;
    min-width: 100%;
    opacity: 1 !important
}
</style>