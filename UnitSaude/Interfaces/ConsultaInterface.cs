namespace UnitSaude.Interfaces
{
    using UnitSaude.Dto.Consulta;
    using UnitSaude.Models;
    public interface ConsultaInterface
    {
        public Task<ResponseModel<object>> CadastrarConsulta(CreateConsultaDto consulta);

        public Task<ResponseModel<Consulta>> ListarConsultaPorId(int ConsultaId);
        public Task<ResponseModel<List<Consulta>>> ListarConsultaPorPaciente(Paciente paciente);
        public Task<ResponseModel<List<Consulta>>> ListarConsultaPorProfessor(Professor professor);
        public Task<ResponseModel<object>> GerenciarConsulta(UpdateConsultaDto consulta, int ConsultaId);
        
    }
}