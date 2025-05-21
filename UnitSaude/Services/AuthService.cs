using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UnitSaude.Models;
using DotNetEnv;
using UnitSaude.Dto.Usuario;

namespace UnitSaude.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        private readonly byte[] _key;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;

            var key = _configuration["Jwt:Key"]
                   ?? Environment.GetEnvironmentVariable("JWT_KEY");

            if (string.IsNullOrWhiteSpace(key))
                throw new Exception("A chave JWT não foi encontrada.");

            _key = Encoding.UTF8.GetBytes(key);
        }

        public string GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, usuario.nome),
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id_Usuario.ToString()),
                    new Claim(ClaimTypes.Role, usuario.TipoUsuario)
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                Issuer = _configuration["Jwt:Issuer"] ?? Environment.GetEnvironmentVariable("JWT_ISSUER"),
                Audience = _configuration["Jwt:Audience"] ?? Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GerarTokenRecuperacao(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email),
                new Claim("tipo", "recuperacao")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(10),
                Issuer = _configuration["Jwt:Issuer"] ?? Environment.GetEnvironmentVariable("JWT_ISSUER"),
                Audience = _configuration["Jwt:Audience"] ?? Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
