using Microsoft.AspNetCore.Mvc;
using UnitSaude.Dto.Paciente;
using UnitSaude.Models;

namespace UnitSaude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PacienteController : ControllerBase
    {
        [HttpPost("CreatePaciente")]
        public async Task<ActionResult<ResponseModel<object>>> CadastrarPaciente(CreatePacienteDto paciente)
        {
            // Implementation for creating Paciente
            return Ok();
        }
        [HttpGet("GetPaciente")]
        public async Task<ActionResult<ResponseModel<ReadPacienteDto>>> ListarPaciente(int PacienteId){
            // Implementation for getting Paciente by ID
            return Ok();
        }
        [HttpGet("GetPacientesEmFilaDeEspera")]
        public async Task<ActionResult<ResponseModel<List<ReadPacienteDto>>>> ListarPacientesEmFilaDeEspera(){
            // Implementation for getting Pacientes in waiting list
            return Ok();
        }
        [HttpPatch("UpdatePaciente")]
        public async Task<ActionResult<ResponseModel<object>>> GerenciarPaciente(UpdatePacienteDto paciente, int PacienteId)
        {
            // Implementation for updating Paciente
            return Ok();
        }
        [HttpDelete("DeletePaciente")]
        public async Task<ActionResult<ResponseModel<object>>> RemoverPaciente(int PacienteId)
        {
            // Implementation for deleting Paciente
            return Ok();
        }
    }
}