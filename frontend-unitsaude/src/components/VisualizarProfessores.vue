<template>
    <Header/>
    <div class="professores">
        <BackButton class="voltar"/>
        <div class=area-professores>
            <h5 id=title><b>Cadastro de professores</b></h5>
            <div id=form-professor class=mb-3>
                <label for="procura-professor">
                    <input
                      type="search"
                      class="form-label"
                      id="procura-professor"
                      placeholder="Digite o nome ou especialidade do professor"
                      v-model="professor"
                      @input="procurarProfessores"
                    />
                    <RouterLink
                    	to="/cadastroProfessor"
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
              id="professores-data"
              v-if="response.data"
              v-for="(professorData, idx) in response.data"
              :key="professorData"
            >
              <div
                v-for="professorInfo in professorData"
                :key="professorInfo"
              >
                <div
                    class="professor"
                    v-if="professorInfo.nome"
                >
                    {{ professorInfo.nome }}
                    {{ professorInfo.area }}
                   	{{ professorInfo.especialidade }}
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
  name: "VisualizarProfessores",
  data() {
    return {
      professor: "",
      professores: [],
      error: "",
      response: {}
    };
  },
  components: {
    Header,
    BackButton,
  },
  mounted() {
    this.procurarProfessores();
  },
  methods: {
    capitalizar(str) {
      if (!str) return "";
      return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
    },
    async procurarProfessores() {
	  this.error = "";

	  try {
	    if (this.professor === "") {
	      this.response = await api.get("/api/Professor/ListarTodos");
	      if (!this.response.status) {
	      	this.error = "Professores não encontrados.";
	      }
	      return;
	    }

	    this.response = await api.get("/api/Professor/ListarComFiltro", {
	      params: {
	        nome: this.capitalizar(this.professor),
	        especialidade: this.professor
	      },
	    });
	    if (this.response.data === null) {
	      	this.error = "Professores não encontrados.";
	      }
	  } catch (error) {
	    this.error = "Professor não foi encontrado.";
	  }
	}
  }
}
</script>

<style>
.professores {
    padding: 120px 20px;
    display: flex;
    justify-content: flex-start;
}
.area-professores {
    padding-left: 5dvw;
}
#form-professor{
    display: flex;
    flex-direction: column;
    gap: 20px
}
#title {
    padding-bottom: 20px
}
#procura-professor {
    min-width: 35dvw;
    padding: 3px
}
.btn--cadastrar {
    position: relative;
    left: 50px;
    background-color: #186fc0;
    color: white;
}
#professores-data {
    display: flex;
    flex-direction: column;
    gap: 15px
}
.professor {
    width: 60dvw;
    box-shadow: 1px 1px 1px 1px;
    border-radius: 9px;
    padding: 15px
}
.professor:hover {
    background-color: #ddd;
    cursor: pointer
}
</style>