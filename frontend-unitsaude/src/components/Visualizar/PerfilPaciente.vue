<template>
  <div id="perfil-paciente">
      <h3>Perfil - {{ paciente.nome }}</h3>
      <div id="area-perfil">
        <i id="icon" class="bi bi-person" :title="`Foto de ${paciente.nome}`" aria-label="Foto do paciente"></i>
        <div id="paciente-info">
          <label for="nome">
            <span>Nome</span>
            <input class="form-control" id="nome" :value="paciente.nome" disabled />
          </label>
          <label for="email">
            <span>E-mail</span>
            <input class="form-control" type="email" id="email" :value="paciente.email" disabled />
          </label>
          <label for="telefone">
            <span>Telefone</span>
            <input class="form-control" id="telefone" :value="paciente.telefone" disabled />
          </label>
          <label for="cpf">
            <span>CPF</span>
            <input class="form-control" id="cpf" :value="paciente.cpf" disabled />
          </label>
          <label for="data-nascimento">
            <span>Data de nascimento</span>
            <input class="form-control" type="date" id="nascimento" :value="paciente.dataNascimento" disabled />
          </label>
        </div>
      </div>
      <div id="area-consultas">
        <h4>Histórico de consultas</h4>

        <div
          v-if="consultas"
          v-for="consultaInfo in consultas"
          :key="consultaInfo.id_Consultas">
          <ConsultaView :consulta="consultaInfo"/>
        </div>

        <span
          v-else
          class="aviso"
          >{{ erro }}</span>
      </div>
    </div>
</template>

<script>
import { useUsuarioStore } from "@/store/usuario";
import api from "@/services/api";
import ConsultaView from "@/components/Consulta";

export default {
  components: {
    ConsultaView
  },
  data() {
    return {
      paciente: {
        nome: '',
        email: '',
        telefone: '',
        cpf: '',
        dataNascimento: ''
      },
      consultas: {},
      erro: ""
    };
  },
  methods: {
    carregaPerfil() {
      const useUsuario = useUsuarioStore();
      this.paciente = useUsuario.getUsuario();
    },
    async carregaConsultas() {
        try {
            if (this.paciente && this.paciente.id) {
                let response = await api.get("api/Consulta/GetConsultaPorPaciente/" + this.paciente.id);

                this.consultas = response.data.data;

                if(!response.status) {
                    this.erro = response.message;
                }
            }
        } catch (error) {
            this.erro = error;
        }        
    }
  },
  mounted() {
    this.carregaPerfil();
    this.carregaConsultas();
  }
};
</script>
<style scoped>
#perfil-paciente, #area-perfil, #paciente-info {
    display: flex;
}
#perfil-paciente {
    flex-direction: column;
    padding: 8rem 3rem;
    gap: 50px
}
#area-perfil {
    align-items:flex-start;
    gap: 10dvw;
}
#paciente-info {
    gap: 10dvw
}
#icon {
    font-size: 22dvw;
    border: solid black 0.1px
}
#paciente-info {
    flex-direction: column;
    justify-content: center;
    gap: 20px
}
label {
    padding: 5px 0px
}
form-control {
    min-width: 100%
}
</style>