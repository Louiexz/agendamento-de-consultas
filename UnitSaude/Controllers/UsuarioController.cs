using Microsoft.AspNetCore.Mvc;
using UnitSaude.Dto.User;
using UnitSaude.Models;

namespace UnitSaude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<ResponseModel<Usuario>>> AutenticarUsuario(LoginDto login){
            // Implementação do método de autenticação
            // Aqui você pode chamar o serviço de autenticação e retornar o resultado
            return Ok();
        }
        [HttpPost("gerar-token")]
        public async Task<ActionResult<ResponseModel<string>>> GerarTokenJwt(){
            // Implementação do método de geração de token JWT
            // Aqui você pode chamar o serviço de geração de token e retornar o resultado
            return Ok();
        }
        [HttpPost("deslogar")]
        public async Task<ActionResult<ResponseModel<bool>>> DeslogarUsuario(string token){
            // Implementação do método de deslogar usuário
            // Aqui você pode chamar o serviço de deslogar e retornar o resultado
            return Ok();
        }
    }
}