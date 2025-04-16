using System.Text.Json.Serialization;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Professor
{
    public class ReadProfessorDto
    {
        public int id { get; set; }
        public required string cpf { get; set; }
        public required string nome { get; set; }
        public required string email { get; set; }
        public required string telefone { get; set; }
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly? dataNascimento { get; set; }
        public required string area { get; set; }
        public required string especialidade { get; set; }
        public required string codigoProfissional { get; set; }
    }
}