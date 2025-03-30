namespace UnitSaude.Dto.User
{
    public class LoginDto
    {
        public required string Credential { get; set; }
        public required string Password { get; set; }
    }
}