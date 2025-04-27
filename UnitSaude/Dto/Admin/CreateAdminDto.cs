using System.Text.Json.Serialization;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Admin
{
    public class CreateAdminDto
    {
        public required string cpf { get; set; }
        public required string nome { get; set; }
        public required string email { get; set; }
        public required string senhaHash { get;  set; }
        public required string telefone { get; set; }

        public DateOnly? dataNascimento { get; set; }
    }
}