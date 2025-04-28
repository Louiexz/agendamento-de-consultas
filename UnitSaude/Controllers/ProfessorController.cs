using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnitSaude.Dto.Paciente;
using UnitSaude.Dto.Professor;
using UnitSaude.Interfaces;
using UnitSaude.Models;
using UnitSaude.Services;

namespace UnitSaude.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorController : ControllerBase
    {
        private readonly ProfessorInterface _professorService;

        public ProfessorController(ProfessorInterface professorService)
        {
            _professorService = professorService;
        }
        [Authorize(Policy = "Administrador")]
        [HttpPost("CreateProfessor")]
        public async Task<ActionResult<ResponseModel<ReadProfessorDto>>> CadastrarProfessor([FromBody] CreateProfessorDto professor)
        {
            var response = await _professorService.CadastrarProfessor(professor);
            if (!response.Status) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<ReadProfessorDto>>> ListarProfessor(int id)
        {
            var response = await _professorService.ListarProfessor(id);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("ListarTodos")]
        public async Task<ActionResult<ResponseModel<List<ReadProfessorDto>>>> ListarTodos()
        {
            var response = await _professorService.ListarProfessores();
            return Ok(response);
        }
        [Authorize(Policy = "Professor")]
        [HttpPatch("Update/{id}")]
        public async Task<ActionResult<ResponseModel<ReadProfessorDto>>> GerenciarProfessor(int id, [FromBody] UpdateProfessorDto professor)
        {
            var response = await _professorService.GerenciarProfessor(professor, id);
            if (!response.Status) return NotFound(response);
            return Ok(response);
        }
        [Authorize(Policy = "Administrador")]
        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ResponseModel<Professor>>> RemoverProfessor(int id)
        {
            var response = await _professorService.RemoverProfessor(id);
            if (!response.Status) return NotFound(response);
            return Ok(response);
        }
        [Authorize(Policy = "Professor")]
        [HttpPatch("AlterarSenha/{professorId}")]
        public async Task<ActionResult<ResponseModel<string>>> AlterarSenha(int professorId, [FromBody] UpdateSenhaProfessorDto dto)
        {
            var resultado = await _professorService.AlterarSenhaProfessor(professorId, dto);

            if (!resultado.Status) return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpGet("listar-professores-especialidade")]
        public async Task<ActionResult<ResponseModel<List<ReadProfessorDto>>>> ListarProfessoresPorEspecialidade([FromQuery] string especialidade)
        {
            var result = await _professorService.ListarProfessoresPorEspecialidade(especialidade);
            if (result.Status)
                return Ok(result);
            else
                return BadRequest(result);
        }

    }
}