using Newtonsoft.Json;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Consulta
{
    public class CreateConsultaDto
    {
        public DateOnly? Data { get; set; }
        public TimeOnly? Horario { get; set; }
        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
        public required string Status { get; set; }
        public required string Area { get; set; }
        public required string Especialidade { get; set; }
        public required string Anamnese { get; set; }
        public int PacienteId { get; set; }
        public int ProfessorId { get; set; }
    }
}