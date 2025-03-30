using Microsoft.AspNetCore.Mvc;
using UnitSaude.Dto.Consulta;
using UnitSaude.Models;

namespace UnitSaude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConsultaController : ControllerBase
    {
        [HttpPost("CreateConsulta")]
        public async Task<ActionResult<ResponseModel<object>>> CreateConsulta([FromBody] CreateConsultaDto Consulta)
        {
            // Implementation for creating Consulta
            return Ok();
        }
        [HttpGet("GetConsulta")]
        public async Task<ActionResult<ResponseModel<Consulta>>> ListarConsultaPorId(int ConsultaId)
        {
            // Implementation for getting Consulta by ID
            return Ok();
        }
        [HttpGet("GetConsultaPorPaciente/{paciente_id}")]
        public async  Task<ActionResult<ResponseModel<List<Consulta>>>> ListarConsultaPorPaciente(Paciente paciente)
        {
            // Implementation for getting Consulta by Paciente
            return Ok();
        }
        [HttpGet("GetConsultaPorProfessor/{professor_id}")]
        public async Task<ActionResult<ResponseModel<List<Consulta>>>> ListarConsultaPorProfessor(Professor professor)
        {
            // Implementation for getting Consulta by Professor
            return Ok();
        }
        [HttpPatch("UpdateConsulta")]
        public async Task<ActionResult<ResponseModel<object>>> UpdateConsulta([FromBody] UpdateConsultaDto Consulta, int ConsultaId)
        {
            // Implementation for updating Consulta
            return Ok();
        }
    }
}