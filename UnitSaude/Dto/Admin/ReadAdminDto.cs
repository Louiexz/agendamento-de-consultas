using Newtonsoft.Json;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Admin
{
    public class ReadAdminDto
    {
        public int id { get; set; }
        public required string cpf { get; set; }
        public required string nome { get; set; }
        public required string email { get; set; }
        public string? telefone { get; set; }

        public DateOnly? dataNascimento { get; set; }
    }
}