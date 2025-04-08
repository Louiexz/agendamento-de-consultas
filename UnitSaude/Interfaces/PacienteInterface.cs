namespace UnitSaude.Interfaces
{
    using UnitSaude.Dto.Paciente;
    using UnitSaude.Models;
    public interface PacienteInterface
    {
        public Task<ResponseModel<ReadPacienteDto>> CadastrarPaciente(CreatePacienteDto paciente);
        public Task<ResponseModel<List<ReadPacienteDto>>> ListarPacientes();
        public Task<ResponseModel<ReadPacienteDto>> ListarPaciente(int PacienteId);
        public Task<ResponseModel<ReadPacienteDto>> GerenciarPaciente(UpdatePacienteDto paciente, int PacienteId);
        public Task<ResponseModel<string>> AlterarSenhaPaciente(int pacienteId, UpdateSenhaPacienteDto dto);
        public Task<ResponseModel<Paciente>> RemoverPaciente(int PacienteId);
    }
}