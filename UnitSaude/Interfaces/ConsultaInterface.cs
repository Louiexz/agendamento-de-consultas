namespace UnitSaude.Interfaces
{
    using UnitSaude.Dto.Consulta;
    using UnitSaude.Dto.Paciente;
    using UnitSaude.Models;
    public interface ConsultaInterface
    {
        public Task<ResponseModel<ReadConsultaDto>> CadastrarConsulta(CreateConsultaDto consulta);
        public Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultas();
        public Task<ResponseModel<ReadConsultaDto>> ListarConsultaPorId(int ConsultaId);
        public Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultaPorStatus(string status);
        public Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultaPorPaciente(int pacienteId);
        public Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultasPorNomeOuCpf(string valor);

        public Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultaPorProfessor(int professorID);
        public Task<ResponseModel<List<string>>> ObterHorariosDisponiveis(DateOnly data, string area, string especialidade);

        public Task<ResponseModel<string>> AtualizarStatusConsulta(int id, UpdateStatusConsultaDto dto);

        //public Task<ResponseModel<ReadConsultaDto>> GerenciarConsulta(UpdateConsultaDto consulta, int ConsultaId);

    }
}