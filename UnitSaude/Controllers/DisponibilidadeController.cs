using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnitSaude.Dto.Disponibilidade;
using UnitSaude.Interfaces;
using UnitSaude.Models;

namespace UnitSaude.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DisponibilidadeController : ControllerBase
    {
        private readonly DisponibilidadeInterface _disponibilidadeService;

        public DisponibilidadeController(DisponibilidadeInterface disponibilidadeService)
        {
            _disponibilidadeService = disponibilidadeService;
        }

        [HttpPost("CreateDisponibilidade")]
        public async Task<ActionResult<ResponseModel<ReadDisponibilidadeDto>>> CadastrarDisponibilidade([FromBody] CreateDisponibilidadeDto dto)
        {
            var response = await _disponibilidadeService.CadastrarDisponibilidade(dto);
            if (!response.Status) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("GetTodasDisponibilidades")]
        public async Task<ActionResult<ResponseModel<List<ReadDisponibilidadeDto>>>> ListarDisponibilidades()
        {
            var response = await _disponibilidadeService.ListarDisponibilidades();
            if (!response.Status) return BadRequest(response);
            return Ok(response);
        }

    }
}
