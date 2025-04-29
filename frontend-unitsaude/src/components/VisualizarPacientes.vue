<template>
    <Header/>
    <div class="pacientes">
        <BackButton class="voltar"/>
        <div class=area-pacientes>
            <h5 id=title><b>Cadastro de pacientes</b></h5>
            <div id=form-paciente class=mb-3>
                <label for="procura-paciente">
                    <input
                      type="search"
                      class="form-label"
                      id="procura-paciente"
                      placeholder="Digite o nome ou CPF do paciente"
                      v-model="paciente"
                      @input="procurarPaciente"
                    />
                    <RouterLink
                        to="/cadastroPaciente"
                        class="btn btn--cadastrar"
                        >
                        <i class="bi bi-plus-lg"></i>
                    </RouterLink>
                </label>
                <span
                    v-if='error != ""'
                    class="alert alert-danger"
                >Erro: {{ error }}</span>
            </div>
            <div
              id="pacientes-data"
              v-if="response.data"
              v-for="(pacienteData, idx) in response.data"
              :key="pacienteData"
            >
              <div
                v-for="pacienteInfo in pacienteData"
                :key="pacienteInfo"
              >
                <div
                    class="paciente"
                    v-if="pacienteInfo.nome"
                >
                    {{ pacienteInfo.nome }}
                </div>
               </div>
            </div>
        </div>
    </div>
</template>
<script>
import BackButton from '@/components/btnVoltar.vue';
import Header from '@/components/Header.vue';
import api from "@/services/api";

export default {
    name: "VisualizarPacientes",
    data() {
        return {
            paciente: "",
            pacientes: [],
            error: "",
            response: {}
        }
    },
    components: {
        Header,
        BackButton,
    },
    setup() {

    },
    methods: {
      capitalizar(str) {
        if (!str) return "";
        return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
      },
      async procurarPaciente() {
        this.error = "";

        try {
          const isCpf = /^\d+$/.test(this.paciente);

          if (this.paciente === "") {
            this.response = await api.get("/api/Paciente/ListarTodos");
            return;
          }

          this.response = await api.get("/api/Paciente/ListarComFiltro", {
            params: {
              nome: isCpf ? "" : this.capitalizar(this.paciente),
              cpf: isCpf ? this.paciente : "",
            },
          });
          if (this.response.data === null) {
            this.error = "Pacientes não encontrados.";
          }
        } catch (error) {
          this.error = "Paciente não foi encontrado.";
        }
      }
    },
    mounted() {
        this.procurarPaciente();
    }
}
</script>
<style>
.pacientes {
    padding: 120px 20px;
    display: flex;
    justify-content: flex-start;
}
.area-pacientes {
    padding-left: 5dvw;
}
#form-paciente {
    display: flex;
    flex-direction: column;
    gap: 20px
}
#title {
    padding-bottom: 20px
}
#procura-paciente {
    min-width: 35dvw;
    padding: 3px
}
.btn--cadastrar {
    position: relative;
    left: 50px;
    background-color: #186fc0;
    color: white;
}
#pacientes-data {
    display: flex;
    flex-direction: column;
    gap: 15px
}
.paciente {
    width: 60dvw;
    box-shadow: 1px 1px 1px 1px;
    border-radius: 9px;
    padding: 15px
}
.paciente:hover {
    background-color: #ddd;
    cursor: pointer
}
</style>