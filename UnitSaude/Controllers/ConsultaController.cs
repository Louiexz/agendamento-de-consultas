using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnitSaude.Dto.Consulta;
using UnitSaude.Interfaces;
using UnitSaude.Models;

namespace UnitSaude.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultaController : ControllerBase
    {
        private readonly ConsultaInterface _consultaService;

        public ConsultaController(ConsultaInterface consultaService)
        {
            _consultaService = consultaService;
        }

        [HttpPost("CreateConsulta")]
        public async Task<ActionResult<ResponseModel<ReadConsultaDto>>> CadastrarConsulta([FromBody] CreateConsultaDto consulta)
        {
            var response = await _consultaService.CadastrarConsulta(consulta);
            if (!response.Status) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("GetConsultaPorId/{consultaId}")]
        public async Task<ActionResult<ResponseModel<ReadConsultaDto>>> ListarConsultaPorId(int consultaId)
        {
            var response = await _consultaService.ListarConsultaPorId(consultaId);
            if (!response.Status) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("GetConsultaPorPaciente/{pacienteId}")]
        public async Task<ActionResult<ResponseModel<List<ReadConsultaDto>>>> ListarConsultaPorPaciente(int pacienteId)
        {
            var response = await _consultaService.ListarConsultaPorPaciente(pacienteId);
            if (!response.Status) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("GetConsultaPorProfessor/{professorId}")]
        public async Task<ActionResult<ResponseModel<List<ReadConsultaDto>>>> ListarConsultaPorProfessor(int professorId)
        {
            var response = await _consultaService.ListarConsultaPorProfessor(professorId);
            if (!response.Status) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("GetConsultaPorStatus/{status}")]
        public async Task<ActionResult<ResponseModel<List<ReadConsultaDto>>>> ListarConsultaPorStatus(string status)
        {
            var response = await _consultaService.ListarConsultaPorStatus(status);
            if (!response.Status) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("GetConsultaPorNomeOuCpf/{valor}")]
        public async Task<ActionResult<ResponseModel<List<ReadConsultaDto>>>> ListarConsultaPorNomeOuCpf(string valor)
        {
            var response = await _consultaService.ListarConsultasPorNomeOuCpf(valor);
            if (!response.Status) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("GetTodasConsultas")]
        public async Task<ActionResult<ResponseModel<List<ReadConsultaDto>>>> ListarConsultas()
        {
            var response = await _consultaService.ListarConsultas();
            if (!response.Status) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("horarios-disponiveis")]
        public async Task<ActionResult<ResponseModel<List<string>>>> ObterHorariosDisponiveis(
            [FromQuery] DateOnly data,
            [FromQuery] string area,
            [FromQuery] string especialidade)
        {
            var response = await _consultaService.ObterHorariosDisponiveis(data, area, especialidade);
            return Ok(response);
        }



    }
}
