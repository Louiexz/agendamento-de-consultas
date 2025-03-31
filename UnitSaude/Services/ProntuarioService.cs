using UnitSaude.Data;
using UnitSaude.Dto.Prontuario;
using UnitSaude.Interfaces;
using UnitSaude.Models;

namespace UnitSaude.Services
{
    public class ProntuarioService : ProntuarioInterface
    {
        private readonly ClinicaDbContext _context;
        public ProntuarioService(ClinicaDbContext context)
        {
            _context = context;
        }

        public Task<ResponseModel<object>> CadastrarProntuario(CreateProntuarioDto prontuario)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<object>> GerenciarProntuario(CreateProntuarioDto prontuario, int prontuarioId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<Prontuario>> ListarProntuarioPorConsulta(int consultaId)
        {
            throw new NotImplementedException();
        }
    }
}