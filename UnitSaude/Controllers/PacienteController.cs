using Microsoft.AspNetCore.Mvc;
using UnitSaude.Dto.Paciente;
using UnitSaude.Interfaces;
using UnitSaude.Models;

namespace UnitSaude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacienteController : ControllerBase
    {
        private readonly PacienteInterface _pacienteService;

        public PacienteController(PacienteInterface pacienteService)
        {
            _pacienteService = pacienteService;
        }

        [HttpPost("CreatePaciente")]
        public async Task<ActionResult<ResponseModel<ReadPacienteDto>>> CadastrarPaciente([FromBody] CreatePacienteDto paciente)
        {
            var response = await _pacienteService.CadastrarPaciente(paciente);
            if (!response.Status) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<ReadPacienteDto>>> ListarPaciente(int id)
        {
            var response = await _pacienteService.ListarPaciente(id);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("ListarTodos")]
        public async Task<ActionResult<ResponseModel<List<ReadPacienteDto>>>> ListarTodos()
        {
            var response = await _pacienteService.ListarPacientes();
            return Ok(response);
        }

        [HttpPatch("Update/{id}")]
        public async Task<ActionResult<ResponseModel<ReadPacienteDto>>> GerenciarPaciente(int id, [FromBody] UpdatePacienteDto paciente)
        {
            var response = await _pacienteService.GerenciarPaciente(paciente, id);
            if (!response.Status) return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ResponseModel<Paciente>>> RemoverPaciente(int id)
        {
            var response = await _pacienteService.RemoverPaciente(id);
            if (!response.Status) return NotFound(response);
            return Ok(response);
        }

        [HttpPatch("AlterarSenha/{pacienteId}")]
        public async Task<ActionResult<ResponseModel<string>>> AlterarSenha(int pacienteId, [FromBody] UpdateSenhaPacienteDto dto)
        {
            var resultado = await _pacienteService.AlterarSenhaPaciente(pacienteId, dto);

            if (!resultado.Status) return BadRequest(resultado);

            return Ok(resultado);
        }

    }
}