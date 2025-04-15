using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UnitSaude.Data;
using UnitSaude.Dto.User;
using UnitSaude.Dto.Usuario;
using UnitSaude.Interfaces;
using UnitSaude.Models;
using UnitSaude.Services;
using UnitSaude.Utils;

namespace UnitSaude.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly ClinicaDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ProfessorInterface _professorInterface;
        private readonly PacienteInterface _pacienteInterface;
        private readonly AdminInterface _administradorInterface;

        public UsuarioController(ClinicaDbContext context, IConfiguration configuration, ProfessorInterface professorInterface, PacienteInterface pacienteInterface, AdminInterface adminInterface)
        {
            _context = context;
            _configuration = configuration;
            _professorInterface = professorInterface;
            _pacienteInterface = pacienteInterface;
            _administradorInterface = adminInterface;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            Usuario? usuario = await _context.Professores.FirstOrDefaultAsync(p => p.email == dto.Credential);
            if (usuario == null)
                usuario = await _context.Pacientes.FirstOrDefaultAsync(p => p.email == dto.Credential);
            if (usuario == null)
                usuario = await _context.Administradores.FirstOrDefaultAsync(a => a.email == dto.Credential);

            if (usuario == null)
                return Unauthorized(new { message = "Credenciais inválidas." });

            if (!PasswordHasher.VerifyPassword(dto.Password, usuario.senhaHash))
                return Unauthorized(new { message = "Credenciais inválidas." });

            var token = AuthService.GerarToken(usuario, _configuration);

            return Ok(new
            {
                token,
                usuario = new
                {
                    usuario.Id_Usuario,
                    usuario.nome,
                    usuario.TipoUsuario
                }
            });
        }

        [HttpPost("recuperar-senha")]
        public async Task<IActionResult> RecuperarSenha([FromBody] RecuperarSenhaDto dto)
        {
            Usuario? usuario =
               await _context.Professores.FirstOrDefaultAsync(p => p.email == dto.Email) as Usuario
               ?? await _context.Pacientes.FirstOrDefaultAsync(p => p.email == dto.Email) as Usuario
               ?? await _context.Administradores.FirstOrDefaultAsync(a => a.email == dto.Email) as Usuario;

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            var token = AuthService.GerarTokenRecuperacao(usuario, _configuration);
            var baseUrl = _configuration["EmailSettings:UrlRedefinicaoSenha"];
            var link = $"{baseUrl}?token={token}";

            var emailService = new EmailService(_configuration);
            await emailService.EnviarAsync(dto.Email, "Recuperação de Senha", $"Clique aqui para redefinir sua senha: <a href='{link}'>Redefinir Senha</a>");

            return Ok("E-mail de recuperação enviado com sucesso.");
        }

        [HttpPost("redefinir-senha")]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaDto dto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            try
            {
                var principal = tokenHandler.ValidateToken(dto.Token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                var tipo = principal.Claims.FirstOrDefault(c => c.Type == "tipo")?.Value;
                if (tipo != "recuperacao")
                    return BadRequest("Token inválido.");

                var idUsuario = int.Parse(principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                var senhaHash = PasswordHasher.HashPassword(dto.NovaSenha);


                var senhaAlterada =

                    await _professorInterface.ResetarSenhaProfessor(idUsuario, senhaHash) ||
                    await _pacienteInterface.ResetarSenhaPaciente(idUsuario, senhaHash) ||
                    await _administradorInterface.ResetarSenhaAdministrador(idUsuario, senhaHash);

                if (!senhaAlterada)
                    return NotFound("Usuário não encontrado.");

                return Ok("Senha redefinida com sucesso.");
            }
            catch
            {
                return BadRequest("Token inválido ou expirado.");
            }
            }
        }
}