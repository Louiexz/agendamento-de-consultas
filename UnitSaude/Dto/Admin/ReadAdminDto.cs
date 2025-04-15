using Newtonsoft.Json;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Admin
{
    public class ReadAdminDto
    {
        public int id { get; set; }
        public string cpf { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string telefone { get; set; }

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly? dataNascimento { get; set; }
    }
}