<template>
  <div class="container py-5 shadow p-5" v-if="usuario">
    <div class="title mb-4">
      <BackButton />
      <h3>Perfil - {{ usuario.nome }}</h3>
    </div>
    <div class="d-flex flex-wrap align-items-center gap-5">
      <i
        class="bi bi-person border border-dark p-3"
        :title="`Foto de ${usuario.nome}`"
        aria-label="Foto do usuario"
        style="font-size: 10rem"
      ></i>

      <form class="d-flex flex-column gap-3 col-5">
        <div>
          <label for="nome" class="form-label">Nome</label>
          <input
            type="text"
            class="form-control"
            id="nome"
            :value="usuario.nome"
            disabled
          />
        </div>

        <div>
          <label for="email" class="form-label">E-mail</label>
          <input
            type="email"
            class="form-control"
            id="email"
            :value="usuario.email"
            disabled
          />
        </div>

        <!-- Por este: -->
        <div v-if="isProfessor">
          <label class="form-label">Especialidades</label>
          <div class="especialidades-container">
            <template
              v-if="usuario.especialidades && usuario.especialidades.length"
            >
              <span
                v-for="(especialidade, index) in usuario.especialidades"
                :key="index"
                class="especialidade-badge"
              >
                {{ especialidade }}
              </span>
            </template>
            <span v-else class="text-muted"
              >Nenhuma especialidade cadastrada</span
            >
          </div>
        </div>

        <div>
          <label for="telefone" class="form-label">Telefone</label>
          <input
            type="text"
            class="form-control"
            id="telefone"
            :value="usuario.telefone"
            disabled
          />
        </div>

        <div>
          <label for="cpf" class="form-label">CPF</label>
          <input
            type="text"
            class="form-control"
            id="cpf"
            :value="usuario.cpf"
            disabled
          />
        </div>

        <div>
          <label for="nascimento" class="form-label">Data de nascimento</label>
          <input
            type="date"
            class="form-control"
            id="nascimento"
            :value="usuario.dataNascimento"
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
  props: {
    isProfessor: {
      type: Boolean,
      default: false,
    },
  },
  components: {
    ConsultaView,
    BackButton,
  },
  data() {
    return {
      usuario: {},
      consultas: [],
      erro: "",
    };
  },
  methods: {
    carregaPerfil() {
      const store = useUsuarioStore();

      const usuarioStore = store.getUsuario();

      const especialidades =
        usuarioStore.especialidades ||
        (usuarioStore.especialidade ? [usuarioStore.especialidade] : []);

      this.usuario = {
        ...usuarioStore,
        especialidades: Array.isArray(especialidades)
          ? especialidades
          : [especialidades],
      };
    },
    async carregaConsultas() {
      try {
        let response;

        if (this.usuario && this.usuario?.id && !this.isProfessor) {
          response = await api.get(
            `api/Consulta/GetConsultaPorPaciente/${this.usuario.id}`
          );
        } else if (this.usuario && this.usuario?.id && this.isProfessor) {
          response = await api.get(
            `api/Consulta/GetConsultaPorProfessor/${this.usuario.id}`
          );
        }

        if (response.data.status) {
          this.consultas = response.data.data;
        } else {
          this.erro = response.data.message;
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
.especialidades-container {
  min-height: 38px;
  padding: 0.375rem 0.75rem;
  border: 1px solid #ced4da;
  border-radius: 0.375rem;
}

.especialidade-badge {
  display: inline-block;
  padding: 0.25rem 0.5rem;
  margin: 0.125rem;
  background-color: #e9ecef;
  border-radius: 0.25rem;
  font-size: 0.875rem;
}

.text-muted {
  color: #6c757d;
  font-style: italic;
}
</style>
