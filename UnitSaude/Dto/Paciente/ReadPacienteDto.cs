using System.Text.Json.Serialization;
using UnitSaude.Models;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Paciente
{
    public class ReadPacienteDto
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