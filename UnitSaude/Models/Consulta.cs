using System.Text.Json.Serialization;
using UnitSaude.Utils;

namespace UnitSaude.Models
{
    public class Consulta
    {
        public int id_Consulta { get; set; }
        public DateOnly? Data { get; set; }
        public TimeOnly? Horario { get; set; }
        public required string Status { get; set; }
        public required string Area { get; set; }
        public required string Especialidade { get; set; }


        // Chaves estrangeiras

        public int PacienteId { get; set; }
        public int ProfessorId { get; set; }

        // Navegação
        public required Paciente Paciente { get; set; }
        public required Professor Professor { get; set; }

    }
}
