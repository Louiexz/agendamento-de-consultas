using Newtonsoft.Json;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Professor
{
    public class CreateProfessorDto
    {
        public required string cpf { get; set; }
        public required string nome { get; set; }
        public required string email { get; set; }
        public required string senhaHash { get; set; }
        public required string telefone { get; set; }

        public DateOnly? dataNascimento { get; set; }
        public required string area { get; set; }
        public required string especialidade { get; set; }
        public required string codigoProfissional { get; set; }
    }
}