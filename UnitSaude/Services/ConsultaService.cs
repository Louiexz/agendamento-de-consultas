using UnitSaude.Data;
using UnitSaude.Dto.Consulta;
using UnitSaude.Interfaces;
using UnitSaude.Models;

namespace UnitSaude.Services
{
    public class ConsultaService : ConsultaInterface
    {
        private readonly ClinicaDbContext _context;
        public ConsultaService(ClinicaDbContext context)
        {
            _context = context;
        }

        public Task<ResponseModel<object>> CadastrarConsulta(CreateConsultaDto consulta)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<object>> GerenciarConsulta(UpdateConsultaDto consulta, int ConsultaId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<Consulta>> ListarConsultaPorId(int ConsultaId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<List<Consulta>>> ListarConsultaPorPaciente(Paciente paciente)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<List<Consulta>>> ListarConsultaPorProfessor(Professor professor)
        {
            throw new NotImplementedException();
        }
    }
}