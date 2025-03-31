namespace UnitSaude.Services
{
    using System.Threading.Tasks;
    using UnitSaude.Data;
    using UnitSaude.Dto.Anexo;
    using UnitSaude.Interfaces;
    using UnitSaude.Models;

    public class AnexoService : AnexoInterface
    {
        private readonly ClinicaDbContext _context;
        public AnexoService(ClinicaDbContext context)
        {
            _context = context;
        }

        public Task<ResponseModel<object>> CadastrarAnexo(CreateAnexoDto anexo)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<object>> GerenciarAnexo(UpdateAnexoDto anexo, int AnexoId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<Anexo>> ListarAnexo(int AnexoId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<object>> RemoverAnexo(int AnexoId)
        {
            throw new NotImplementedException();
        }
    }
}