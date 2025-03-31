using UnitSaude.Data;
using UnitSaude.Dto.Paciente;
using UnitSaude.Interfaces;
using UnitSaude.Models;

namespace UnitSaude.Services
{
    public class PacienteService : PacienteInterface
    {
        private readonly ClinicaDbContext _context;
        public PacienteService(ClinicaDbContext context)
        {
            _context = context;
        }

        public Task<ResponseModel<object>> CadastrarPaciente(CreatePacienteDto paciente)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<object>> GerenciarPaciente(UpdatePacienteDto paciente, int PacienteId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<ReadPacienteDto>> ListarPaciente(int PacienteId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<List<ReadPacienteDto>>> ListarPacientesEmFilaDeEspera()
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<object>> RemoverPaciente(int PacienteId)
        {
            throw new NotImplementedException();
        }
    }
}