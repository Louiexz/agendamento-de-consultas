namespace UnitSaude.Interfaces
{
    using UnitSaude.Dto.User;
    using UnitSaude.Models;
    public interface UsuarioInterface
    {
        public Task<ResponseModel<Usuario>> AutenticarUsuario(LoginDto login);
        public Task<ResponseModel<string>> GerarTokenJwt();
        public Task<ResponseModel<bool>> DeslogarUsuario(string token);
    }
}