using System.Text.Json.Serialization;
using UnitSaude.Models;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Paciente
{
    public class ReadPacienteDto
    {
        public int id { get; set; }
        public required string cpf { get; set; }
        public required string nome { get; set; }
        public required string email { get; set; }
        public string? telefone { get; set; }
        public DateOnly? dataNascimento { get; set; }
        public required string cep { get; set; }
        public required string estado { get; set; }
        public required string cidade { get; set; }
    }
}