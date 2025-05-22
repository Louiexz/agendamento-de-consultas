using System.Text.Json.Serialization;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Paciente
{
    public class CreatePacienteDto
    {
        public required string cpf { get; set; }
        public required string nome { get; set; }
        public required string email { get; set; }
        public required string senhaHash { get; set; }
        public required string telefone { get; set; }
        public required string cep { get; set; }
        public required string estado { get; set; }
        public required string cidade { get; set; }
        public DateOnly? dataNascimento { get; set; }
    }
}