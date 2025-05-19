<template>
  <div class="container py-5" v-if="usuario">
    <div class="title mb-4">
      <BackButton />
      <h3>Perfil - {{ usuario.nome }}</h3>
    </div>

    <div class="profile-content">
      <!-- Loading para dados do perfil -->
      <div v-if="isLoadingProfile" class="text-center my-5">
        <div class="spinner-border text-primary" role="status"></div>
        <p class="mt-2">Carregando perfil...</p>
      </div>

      <form class="profile-form" v-else>
        <div class="row">
          <div class="col-md-6">
            <div class="mb-3">
              <label for="nome" class="form-label">Nome</label>
              <input
                type="text"
                class="form-control"
                id="nome"
                :value="usuario.nome"
                disabled
              />
            </div>

            <div class="mb-3">
              <label for="email" class="form-label">E-mail</label>
              <input
                type="email"
                class="form-control"
                id="email"
                :value="usuario.email"
                disabled
              />
            </div>

            <div class="mb-3" v-if="isProfessor">
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
          </div>

          <div class="col-md-6">
            <div class="mb-3">
              <label for="telefone" class="form-label">Telefone</label>
              <input
                type="text"
                class="form-control"
                id="telefone"
                :value="usuario.telefone"
                disabled
              />
            </div>

            <div class="mb-3">
              <label for="cpf" class="form-label">CPF</label>
              <input
                type="text"
                class="form-control"
                id="cpf"
                :value="usuario.cpf"
                disabled
              />
            </div>

            <div class="mb-3">
              <label for="nascimento" class="form-label"
                >Data de nascimento</label
              >
              <input
                type="date"
                class="form-control"
                id="nascimento"
                :value="usuario.dataNascimento"
                disabled
              />
            </div>
          </div>
        </div>
      </form>

    </div>
  </div>
</template>

<script>
import { useUsuarioStore } from "@/store/usuario";
import { useAuthStore } from "@/store/auth";
import api from "@/services/api";
import BackButton from "@/components/btnVoltar.vue";

export default {
  props: {
    isProfessor: {
      type: Boolean,
      default: false,
    },
  },
  components: {
    BackButton,
  },
  data() {
    return {
      usuario: null,
      consultas: [],
      erro: "",
      isLoadingProfile: false,
      isLoadingConsultas: false,
    };
  },
  isOwnAdminProfile() {
    const authStore = useAuthStore();
    return (
      authStore.tipoUsuario === "Administrador" &&
      this.usuario?.id &&
      authStore.id_Usuario &&
      this.usuario.id === authStore.id_Usuario
    );
  },
  methods: {
    async carregaPerfil() {
      this.isLoadingProfile = true;
      const authStore = useAuthStore();
      const usuarioStore = useUsuarioStore();

      try {
        // Verifica se há um usuário específico na store (perfil listado)
        const usuarioStoreData = usuarioStore.getUsuario();

        if (usuarioStoreData && usuarioStoreData.id) {
          // Se tem usuário na store, usa esses dados (perfil listado)
          const especialidades =
            usuarioStoreData.especialidades ||
            (usuarioStoreData.especialidade
              ? [usuarioStoreData.especialidade]
              : []);

          this.usuario = {
            ...usuarioStoreData,
            especialidades: Array.isArray(especialidades)
              ? especialidades
              : [especialidades],
          };
        } else {
          // Se não tem usuário na store, busca do usuário logado
          let endpoint = "";

          if (authStore.tipoUsuario === "Paciente") {
            endpoint = `/api/Paciente/${authStore.id_Usuario}`;
          } else if (authStore.tipoUsuario === "Professor") {
            endpoint = `/api/Professor/${authStore.id_Usuario}`;
          } else if (authStore.tipoUsuario === "Administrador") {
            endpoint = `/api/Admin/${authStore.id_Usuario}`;
          }

          if (endpoint) {
            const response = await api.get(endpoint);
            if (response.data.status) {
              const usuarioData = response.data.data;

              const especialidades =
                usuarioData.especialidades ||
                (usuarioData.especialidade ? [usuarioData.especialidade] : []);

              this.usuario = {
                ...usuarioData,
                especialidades: Array.isArray(especialidades)
                  ? especialidades
                  : [especialidades],
              };
            }
          }
        }
      } catch (error) {
        console.error("Erro ao carregar perfil:", error);
      } finally {
        this.isLoadingProfile = false;
      }
    },

  },
  mounted() {
    this.carregaPerfil();

  },
  watch: {
    "$route.fullPath"() {
      this.carregaPerfil();
    },
  },
};
</script>

<style scoped>
/* Seus estilos existentes permanecem os mesmos */
.container {
  max-width: 900px;
  margin: 2rem auto;
  margin-top: 7rem;
  padding: 2rem;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.title {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 2rem;
}

.title h3 {
  margin: 0;
  color: #2c3e50;
  font-weight: 600;
}

.profile-form {
  padding: 1.5rem;
  background: #f8f9fa;
  border-radius: 6px;
}

.form-control:disabled {
  opacity: 1;
  background: white;
  border-color: #dee2e6;
}

.especialidades-container {
  min-height: 38px;
  padding: 0.375rem 0.75rem;
  border: 1px solid #dee2e6;
  border-radius: 0.375rem;
  background: white;
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
}

.consultas-section {
  border-top: 1px solid #eee;
  padding-top: 1.5rem;
}

.consultas-section h4 {
  color: #2c3e50;
  margin-bottom: 1rem;
}

.no-consultas {
  color: #6c757d;
  font-style: italic;
}

@media (max-width: 768px) {
  .container {
    padding: 1rem;
    margin: 1rem;
  }

  .profile-form {
    padding: 1rem;
  }
}
</style>
