using System.Text.Json.Serialization;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Paciente
{
    public class CreatePacienteDto
    {
        public string cpf { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string senhaHash { get; set; }
        public string telefone { get; set; }

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly? dataNascimento { get; set; }
    }
}