using Newtonsoft.Json;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Consulta
{
    public class ReadConsultaDto
    {
        public int id_Consulta { get; set; }

        public DateOnly? Data { get; set; }

        public TimeOnly? Horario { get; set; }

        public required string Status { get; set; }

        public required string Especialidade { get; set; }
        public required string Area { get; set; }

        public int PacienteId { get; set; }
        public int ProfessorId { get; set; }

        public required string NomePaciente { get; set; }
        public required string NomeProfessor { get; set; }
    }
}
