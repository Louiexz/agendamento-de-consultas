using UnitSaude.Data;
using UnitSaude.Dto.User;
using UnitSaude.Interfaces;
using UnitSaude.Models;

namespace UnitSaude.Services
{
    public class UsuarioService : UsuarioInterface
    {
        public readonly ClinicaDbContext _context;
        public UsuarioService(ClinicaDbContext context)
        {
            _context = context;
        }

        public Task<ResponseModel<Usuario>> AutenticarUsuario(LoginDto login)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<bool>> DeslogarUsuario(string token)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<string>> GerarTokenJwt()
        {
            throw new NotImplementedException();
        }
    }
}