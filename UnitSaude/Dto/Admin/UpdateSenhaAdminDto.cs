namespace UnitSaude.Dto.Admin
{
    public class UpdateSenhaAdminDto
    {
        public required string SenhaAtual { get; set; }
        public required string NovaSenha { get; set; }
    }
}
