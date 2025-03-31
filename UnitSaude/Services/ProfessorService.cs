
using UnitSaude.Data;
using UnitSaude.Dto.Professor;
using UnitSaude.Interfaces;
using UnitSaude.Models;

namespace UnitSaude.Services
{
    public class ProfessorService : ProfessorInterface
    {
        private readonly ClinicaDbContext _context;
        public ProfessorService(ClinicaDbContext context)
        {
            _context = context;
        }

        public Task<ResponseModel<object>> CadastrarProfessor(CreateProfessorDto professor)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<object>> GerenciarProfessor(UpdateProfessorDto professor, int ProfessorId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<ReadProfessorDto>> ListarProfessor(int ProfessorId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<List<ReadProfessorDto>>> ListarProfessoresPorEspecialidade(string especialidade)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<object>> RemoverProfessor(int ProfessorId)
        {
            throw new NotImplementedException();
        }
    }
}