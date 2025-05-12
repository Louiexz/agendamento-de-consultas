namespace UnitSaude.Interfaces
{
    using UnitSaude.Dto.Consulta;
    using UnitSaude.Dto.Paciente;
    using UnitSaude.Dtos.Consulta;
    using UnitSaude.Models;
    public interface ConsultaInterface
    {
        public Task<ResponseModel<ReadConsultaDto>> CadastrarConsulta(CreateConsultaDto consulta);
        public Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultas();
        public Task<ResponseModel<ReadConsultaDto>> ListarConsultaPorId(int ConsultaId);
        // public  Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultaPorStatus(string status, string? area = null, string? especialidade = null, DateOnly? data = null);
        public Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultaComFiltro(FiltroConsultaDto filtro);
        public Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultaPorPaciente(int pacienteId);
        public Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultasPorNomeOuCpf(string valor);

        public Task<ResponseModel<List<ReadConsultaDto>>> ListarConsultaPorProfessor(int professorID);
        public Task<ResponseModel<List<HorarioDisponivelDto>>> ObterHorariosDisponiveis(DateOnly data, string area, string especialidade);

        public Task<ResponseModel<string>> AtualizarStatusConsulta(int id, UpdateStatusConsultaDto dto);
        public  Task<ResponseModel<string>> ReagendarConsulta(int id, ReagendarConsultaDto dto);
        public Task<ResponseModel<List<ConsultasEmEsperaResumoDto>>> ObterResumoConsultasEmEspera();
        public Task<ResponseModel<List<ReadConsultaDto>>> ListarHistoricoPaciente(int pacienteId);

        //public Task<ResponseModel<ReadConsultaDto>> GerenciarConsulta(UpdateConsultaDto consulta, int ConsultaId);

        public Task<ResponseModel<ReadConsultaDto>> ConfirmarConsulta(int consultaId);

    }
}