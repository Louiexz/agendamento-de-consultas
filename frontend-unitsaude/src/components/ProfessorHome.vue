<template>
  <div>
    <!-- Componente Header -->
    <Header/>

    <div
      class="main d-flex min-vh-100 justify-content-center align-items-center"
    >
      <div class="funcoes d-flex flex-column gap-3">
        <router-link
          to="/pacientes"
          class="card align-items-center p-4 no-underline"
        >
          <i class="bi bi-heart-pulse-fill"></i>
          <span>Pacientes</span>
        </router-link>
        <div class="consultas d-flex flex-column gap-3 no-underline">
          <div class="title">
            <h1>Consultas</h1>
          </div>
          <Filtro 
            v-if="userInfo.name" 
            :area="area" 
            :isProfessor="userInfo"
          />
          <div v-else class="loading">
            Carregando consultas do professor...
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import api from "@/services/api";
import Header from "@/components/Header.vue"; // Importando o componente Header
import Filtro from "@/components/FiltrarConsulta.vue"
import { useAuthStore } from "@/store/auth";

export default {
  components: {
    Header, // Registrando o componente Header para uso aqui
    Filtro
  },
  data() {
    return {
      auth: null,
      area: "",
      professor: null,
      userInfo: {status: true}
    }
  },
  async created() {
    this.auth = useAuthStore();
    this.professor = await this.getProfessor();
    this.userInfo = { 
      status: true, 
      name: this.auth.nomeUsuario, 
      professorId: this.professor?.id
    };
    if (this.professor) {
      this.area = this.professor.area;
    }
  },
  methods: {
    async getProfessor() {
      try {
        const res = await api.get("/api/Professor/ListarComFiltro", {
          params: {
            nome: this.auth.nomeUsuario,
          },
        });

        const professors = res.data.data || [];
        if (professors.length > 0) {
          return professors[0];
        }
        return null;
      } catch (error) {
        console.log(error);
        return null;
      }
    }
  },
};
</script>

<style scoped>
.main {
  padding-top: 7rem;
}
.bi {
  color: #186fc0;
  font-size: 40px;
}

span {
  font-size: large;
}

.main {
  gap: 10%;
}

.funcoes {
  width: 40%;
  text-align: center;
  gap: 1rem
}

.consultas {
  width: 30%;
  min-width: 100%
}

.title {
  background-color: #186fc0;
  padding: 10px;
  border-top-left-radius: 15px;
  border-top-right-radius: 15px;
  color: white;
  text-align: center;
}

.title h1 {
  margin: 0;
  font-size: 1.5rem;
}

.card {
  box-shadow: 0 2px 3px rgba(0, 0, 0, 0.2);
  cursor: pointer;
  transition: transform 0.3s ease;
}

.card:hover {
  transform: scale(1.05);
}

@media (max-width: 768px) {
  .main {
    flex-direction: column;
    
  }

  .funcoes {
    width: 70%;
  }

  .consultas {
    width: 70%;
  }

  .main {
    margin-top: 15vh;
    margin-bottom: 25px;
  }
  span {
    font-size: small;
  }
  
}
</style>
