using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnitSaude.Dto.Consulta;
using UnitSaude.Dtos.Consulta;
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

        /*  [HttpGet("GetConsultaPorStatus/{status}")]
          public async Task<ActionResult<ResponseModel<List<ReadConsultaDto>>>> ListarConsultaPorStatus(
              string status,
              [FromQuery] string? area,
              [FromQuery] string? especialidade,
              [FromQuery] DateOnly? data)
          {
              var response = await _consultaService.ListarConsultaPorStatus(status, area, especialidade, data);
              if (!response.Status) return NotFound(response);
              return Ok(response);
          }
        */

        [HttpGet("FiltrarConsultas")]
        public async Task<ActionResult<ResponseModel<List<ReadConsultaDto>>>> FiltrarConsultas([FromQuery] FiltroConsultaDto filtro)
        {
            var response = await _consultaService.ListarConsultaComFiltro(filtro);
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

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> AtualizarStatusConsulta(int id, [FromBody] UpdateStatusConsultaDto dto)
        {
            var resultado = await _consultaService.AtualizarStatusConsulta(id, dto);

            if (!resultado.Status)
                return BadRequest(resultado);

            return Ok(resultado);
        }

        [HttpGet("resumo-fila-espera")]
        public async Task<IActionResult> GetResumoFilaEspera()
        {
            var response = await _consultaService.ObterResumoConsultasEmEspera();
            return response.Status ? Ok(response) : BadRequest(response);
        }

        [HttpPatch("ReagendarConsulta/{id}")]
        public async Task<ActionResult<ResponseModel<string>>> ReagendarConsulta(int id, [FromBody] ReagendarConsultaDto dto)
        {
            var response = await _consultaService.ReagendarConsulta(id, dto);
            if (!response.Status) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("HistoricoPaciente/{pacienteId}")]
        public async Task<ActionResult<ResponseModel<List<ReadConsultaDto>>>> ListarHistoricoPaciente(int pacienteId)
        {
            var response = await _consultaService.ListarHistoricoPaciente(pacienteId);

            if (!response.Status)
            {
                return NotFound(response);  // Retorna 404 se não encontrar histórico
            }

            return Ok(response);  // Retorna 200 com o histórico
        }

        [HttpGet("areas")]
        public ActionResult<ResponseModel<List<string>>> ObterAreas()
        {
            var response = new ResponseModel<List<string>>();

            try
            {
                var areas = DadosFixosConsulta.ObterAreas();
                response.Data = areas;
                response.Message = "Áreas obtidas com sucesso!";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"Erro ao obter áreas: {ex.Message}";
                return StatusCode(500, response);
            }
        }

        [HttpGet("especialidades/{area}")]
        public ActionResult<ResponseModel<List<string>>> ObterEspecialidadesPorArea(string area)
        {
            var response = new ResponseModel<List<string>>();

            try
            {
                var especialidades = DadosFixosConsulta.ObterEspecialidadesPorArea(area);
                if (especialidades == null || !especialidades.Any())
                {
                    response.Status = false;
                    response.Message = $"Nenhuma especialidade encontrada para a área '{area}'.";
                    return NotFound(response);
                }

                response.Data = especialidades;
                response.Message = "Especialidades obtidas com sucesso!";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = $"Erro ao obter especialidades: {ex.Message}";
                return StatusCode(500, response);
            }
        }

        [HttpPost("confirmar/{id}")]
        public async Task<ActionResult<ResponseModel<ReadConsultaDto>>> ConfirmarConsulta(int id)
        {
            var response = await _consultaService.ConfirmarConsulta(id);
            return Ok(response);
        }

        [HttpPatch("CancelarConsulta/{consultaId}")]
        public async Task<ActionResult<ResponseModel<string>>> CancelarConsulta(int consultaId)
        {
            var response = await _consultaService.CancelarConsulta(consultaId);

            if (!response.Status)
            {
                return BadRequest(response); // Retorna 400 com a mensagem de erro
            }

            return Ok(response); // Retorna 200 com a confirmação
        }
    }
}
