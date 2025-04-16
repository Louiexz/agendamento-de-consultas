using System.Text.Json.Serialization;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Professor
{
    public class UpdateProfessorDto
    {
        public string? cpf { get; set; }
        public string? nome { get; set; }
        public string? email { get; set; }
        public string? telefone { get; set; }
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly? dataNascimento { get; set; }
        public string? area { get; set; }
        public string? especialidade { get; set; }
        public string? codigoProfissional { get; set; }
    }
}