using System.Text.Json.Serialization;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Paciente
{
    public class UpdatePacienteDto
    {
        public string cpf { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string telefone { get; set; }
        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly? dataNascimento { get; set; }
    }
}