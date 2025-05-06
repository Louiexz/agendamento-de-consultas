<template>
  <div class="container py-5 shadow p-5">
    <div class="title mb-4">
      <BackButton />
      <h3>Perfil - {{ paciente.nome }}</h3>
    </div>
    <div class="d-flex flex-wrap align-items-center gap-5">
      <i
        class="bi bi-person border border-dark p-3"
        :title="`Foto de ${paciente.nome}`"
        aria-label="Foto do paciente"
        style="font-size: 10rem"
      ></i>

      <form class="d-flex flex-column gap-3 col-5">
        <div>
          <label for="nome" class="form-label">Nome</label>
          <input
            type="text"
            class="form-control"
            id="nome"
            :value="paciente.nome"
            disabled
          />
        </div>

        <div>
          <label for="email" class="form-label">E-mail</label>
          <input
            type="email"
            class="form-control"
            id="email"
            :value="paciente.email"
            disabled
          />
        </div>

        <div>
          <label for="telefone" class="form-label">Telefone</label>
          <input
            type="text"
            class="form-control"
            id="telefone"
            :value="paciente.telefone"
            disabled
          />
        </div>

        <div>
          <label for="cpf" class="form-label">CPF</label>
          <input
            type="text"
            class="form-control"
            id="cpf"
            :value="paciente.cpf"
            disabled
          />
        </div>

        <div>
          <label for="nascimento" class="form-label">Data de nascimento</label>
          <input
            type="date"
            class="form-control"
            id="nascimento"
            :value="paciente.dataNascimento"
            disabled
          />
        </div>
      </form>
    </div>

    <div class="mt-5">
      <h4>Histórico de Consultas</h4>

      <div v-if="consultas?.length">
        <ConsultaView
          v-for="consulta in consultas"
          :key="consulta.id_Consultas"
          :consulta="consulta"
        />
      </div>

      <span v-else class="text-danger">{{ erro }}</span>
    </div>
  </div>
</template>

<script>
import { useUsuarioStore } from "@/store/usuario";
import api from "@/services/api";
import ConsultaView from "@/components/Consulta";
import BackButton from "@/components/btnVoltar.vue";

export default {
  components: {
    ConsultaView,
    BackButton,
  },
  data() {
    return {
      paciente: {
        nome: "",
        email: "",
        telefone: "",
        cpf: "",
        dataNascimento: "",
        id: null,
      },
      consultas: [],
      erro: "",
    };
  },
  methods: {
    carregaPerfil() {
      const store = useUsuarioStore();
      this.paciente = store.getUsuario();
    },
    async carregaConsultas() {
      try {
        if (this.paciente?.id) {
          const response = await api.get(
            `api/Consulta/GetConsultaPorPaciente/${this.paciente.id}`
          );
          if (response.data.status) {
            this.consultas = response.data.data;
          } else {
            this.erro = response.data.message;
          }
        }
      } catch (error) {
        this.erro = "Erro ao carregar consultas.";
        console.error(error);
      }
    },
  },
  mounted() {
    this.carregaPerfil();
    this.carregaConsultas();
  },
};
</script>

<style scoped>

.title {
  text-align: center;
  display: grid;
  grid-template-columns: auto 1fr; /* 2 colunas: uma para a seta e outra para o título */
  align-items: center; /* Alinha os itens verticalmente */
}
h3 {
  margin: 0;
}
.container {
  margin-top: 7rem;
}
.bi-person {
  font-size: 10rem;
}
</style>
