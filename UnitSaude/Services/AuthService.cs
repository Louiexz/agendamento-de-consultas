using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UnitSaude.Models;
using DotNetEnv;

namespace UnitSaude.Services
{
    public class AuthService(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;
        private static byte[] _key;

        static AuthService()
        {
            Env.Load();
            var envKey = Environment.GetEnvironmentVariable("JWT_KEY");
            if (string.IsNullOrWhiteSpace(envKey))
            {
                throw new Exception("Chave JWT não encontrada no .env");
            }

            _key = Encoding.UTF8.GetBytes(envKey);
        }

        public static string GerarToken(Usuario usuario, IConfiguration configuration)
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
                Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
                Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string GerarTokenRecuperacao(Usuario usuario, IConfiguration config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id_Usuario.ToString()),
                new Claim("tipo", "recuperacao")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
