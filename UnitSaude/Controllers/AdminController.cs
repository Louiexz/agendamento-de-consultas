using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnitSaude.Dto.Admin;
using UnitSaude.Dto.Paciente;
using UnitSaude.Interfaces;
using UnitSaude.Models;

namespace UnitSaude.Controllers
{
    [Authorize(Policy = "Administrador")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AdminInterface _adminService;

        public AdminController(AdminInterface adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("CreateAdmin")]
        public async Task<ActionResult<ResponseModel<ReadAdminDto>>> CadastrarAdmin([FromBody] CreateAdminDto admin)
        {
            var response = await _adminService.CadastrarAdmin(admin);
            if (!response.Status) return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseModel<ReadAdminDto>>> ListarAdmin(int id)
        {
            var response = await _adminService.ListarAdmin(id);
            if (response.Data == null) return NotFound(response);
            return Ok(response);
        }

        [HttpGet("ListarTodos")]
        public async Task<ActionResult<ResponseModel<List<ReadAdminDto>>>> ListarTodos()
        {
            var response = await _adminService.ListarAdmins();
            return Ok(response);
        }

        [HttpPatch("Update/{id}")]
        public async Task<ActionResult<ResponseModel<ReadAdminDto>>> GerenciarAdmin(int id, [FromBody] UpdateAdminDto admin)
        {
            var response = await _adminService.GerenciarAdmin(admin, id);
            if (!response.Status) return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<ActionResult<ResponseModel<Administrador>>> RemoverAdmin(int id)
        {
            var response = await _adminService.RemoverAdmin(id);
            if (!response.Status) return NotFound(response);
            return Ok(response);
        }

        [HttpPatch("AlterarSenha/{pacienteId}")]
        public async Task<ActionResult<ResponseModel<string>>> AlterarSenha(int pacienteId, [FromBody] UpdateSenhaAdminDto dto)
        {
            var resultado = await _adminService.AlterarSenhaAdmin(pacienteId, dto);

            if (!resultado.Status) return BadRequest(resultado);

            return Ok(resultado);
        }
    }
}