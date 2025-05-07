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
        public DateOnly? dataNascimento { get; set; }
        public required string area { get; set; }
        public List<string> especialidades { get; set; } = new List<string>(); // Alterado para List<string>
        public required string codigoProfissional { get; set; }
    }
}