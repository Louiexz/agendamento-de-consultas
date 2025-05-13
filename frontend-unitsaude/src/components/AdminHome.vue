<template>
  <div>
    <!-- Componente Header -->
    <Header />

    <div
      class="main d-flex min-vh-100 justify-content-center align-items-center"
    >
      <div class="funcoes d-flex flex-column gap-3">
        <div class="horarios d-flex gap-3 justify-content-center">
          <router-link
            to="/disponibilizar-horarios"
            class="card align-items-center p-4 no-underline w-50"
          >
            <i class="bi bi-calendar-plus-fill"></i>
            <span>Disponibilizar Horários</span>
          </router-link>
          <router-link
            to="/cadastrar-consultas"
            class="card align-items-center p-4 no-underline w-50"
          >
            <i class="bi bi-calendar2-week-fill"></i>
            <span>Cadastrar Consulta</span>
          </router-link>
        </div>
        <router-link
          to="/pacientes"
          class="card align-items-center p-4 no-underline"
        >
          <i class="bi bi-heart-pulse-fill"></i>
          <span>Pacientes</span>
        </router-link>

        <router-link
          to="/professores"
          class="card align-items-center p-4 no-underline"
        >
          <i class="bi bi-bandaid-fill"></i>
          <span>Professores</span>
        </router-link>
      </div>

      <div class="consultas d-flex flex-column gap-3 no-underline">
        <div class="title">
          <h1>Consultas Agendadas</h1>
        </div>

        <router-link to="/consultas/Estetica" class="card p-3 no-underline">
          <span>Estética e Saúde</span>
        </router-link>
        <router-link to="/consultas/Fisioterapia" class="card p-3 no-underline">
          <span>Fisioterapia</span>
        </router-link>
        <router-link to="/consultas/Nutrição" class="card p-3 no-underline">
          <span>Nutrição</span>
        </router-link>
        <router-link to="/consultas/Odontologia" class="card p-3 no-underline">
          <span>Odontologia</span>
        </router-link>
        <router-link to="/consultas/Psicologia" class="card p-3 no-underline">
          <span>Psicologia</span>
        </router-link>
        <router-link to="/consultas/Enfermagem" class="card p-3 no-underline">
          <span>Enfermagem</span>
        </router-link>
      </div>
    </div>
  </div>
</template>

<script>
import Header from "@/components/Header.vue"; // Importando o componente Header

export default {
  components: {
    Header, // Registrando o componente Header para uso aqui
  },
};
</script>

<style scoped>
.main {
  padding-top: 4rem;
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
}

.consultas {
  width: 30%;
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
    gap: 1rem;
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
