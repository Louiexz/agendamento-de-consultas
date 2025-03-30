namespace UnitSaude.Interfaces
{
    using UnitSaude.Dto.Paciente;
    using UnitSaude.Models;
    public interface PacienteInterface
    {
        public Task<ResponseModel<object>> CadastrarPaciente(CreatePacienteDto paciente);
        public Task<ResponseModel<ReadPacienteDto>> ListarPaciente(int PacienteId);
        public Task<ResponseModel<List<ReadPacienteDto>>> ListarPacientesEmFilaDeEspera();
        public Task<ResponseModel<object>> GerenciarPaciente(UpdatePacienteDto paciente, int PacienteId);
        public Task<ResponseModel<object>> RemoverPaciente(int PacienteId);
    }
}