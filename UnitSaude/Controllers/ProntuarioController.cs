using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnitSaude.Dto.Prontuario;
using UnitSaude.Models;

namespace UnitSaude.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProntuarioController : ControllerBase
    {
        [HttpPost("CreateProntuario")]
        public async Task<ActionResult<ResponseModel<object>>> CadastrarProntuario(CreateProntuarioDto prontuario)
        {
            // Implement the logic to create a new prontuario
            return Ok();
        }
        [HttpGet("ListarProntuarioPorConsulta/{consultaId}")]
        public async Task<ActionResult<ResponseModel<Prontuario>>> ListarProntuarioPorConsulta(int consultaId)
        {
            // Implement the logic to list prontuario by consulta ID
            return Ok();
        }
        [HttpPut("GerenciarProntuario/{prontuarioId}")]
        public async Task<ActionResult<ResponseModel<object>>> GerenciarProntuario(CreateProntuarioDto prontuario, int prontuarioId)
        {
            // Implement the logic to manage prontuario by ID
            return Ok();
        }
    }
}