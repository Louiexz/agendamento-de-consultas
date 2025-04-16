namespace UnitSaude.Dto.Usuario
{
    public class RedefinirSenhaDto
    {
        public required string Token { get; set; }
        public required string NovaSenha { get; set; }
    }
}
