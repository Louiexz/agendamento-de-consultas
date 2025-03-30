using Microsoft.AspNetCore.Mvc;
using UnitSaude.Dto.Admin;
using UnitSaude.Models;

namespace UnitSaude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpGet("GetAdmin")]
        public async Task<ActionResult<ResponseModel<ReadAdminDto>>> GetAdmin()
        {
            // Implementation for getting admin
            return Ok();
        }
        [HttpPost("CreateAdmin")]
        public async Task<ActionResult<ResponseModel<object>>> CreateAdmin([FromBody] CreateAdminDto admin)
        {
            // Implementation for creating admin
            return Ok();
        }
        [HttpPatch("UpdateAdmin")]
        public async Task<ActionResult<ResponseModel<object>>> UpdateAdmin([FromBody] UpdateAdminDto admin, int adminId)
        {
            // Implementation for updating admin
            return Ok();
        }
    }
}