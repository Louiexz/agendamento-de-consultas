using Microsoft.AspNetCore.Mvc;
using UnitSaude.Dto.Professor;
using UnitSaude.Models;

namespace UnitSaude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorController : ControllerBase
    {
        [HttpPost("CreateProfessor")]
        public async Task<ActionResult<ResponseModel<object>>> CadastrarProfessor(CreateProfessorDto professor)
        {
            // Implementation for creating Professor
            return Ok();
        }
        [HttpGet("GetProfessor")]
        public async Task<ActionResult<ResponseModel<ReadProfessorDto>>> ListarProfessor(int ProfessorId)
        {
            // Implementation for getting Professor by ID
            return Ok();
        }
        [HttpGet("GetProfessoresPorEspecialidade")]
        public async Task<ActionResult<ResponseModel<List<ReadProfessorDto>>>> ListarProfessoresPorEspecialidade(string especialidade){
            // Implementation for getting Professores by Especialidade
            return Ok();
        }
        [HttpPatch("UpdateProfessor")]
        public async Task<ActionResult<ResponseModel<object>>> GerenciarProfessor(UpdateProfessorDto professor, int ProfessorId)
        {
            // Implementation for updating Professor
            return Ok();
        }
        [HttpDelete("DeleteProfessor")]
        public async Task<ActionResult<ResponseModel<object>>> RemoverProfessor(int ProfessorId)
        {
            // Implementation for deleting Professor
            return Ok();
        }
    }
}