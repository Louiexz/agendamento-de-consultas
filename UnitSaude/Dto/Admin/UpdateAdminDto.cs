using Newtonsoft.Json;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Admin
{
    public class UpdateAdminDto
    {

        public string? cpf { get; set; }
        public string? nome { get; set; }
        public string? email { get; set; }
        public string? telefone { get; set; }

        public DateOnly? dataNascimento { get; set; }
    }
}