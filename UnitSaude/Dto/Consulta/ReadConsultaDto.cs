using Newtonsoft.Json;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Consulta
{
    public class ReadConsultaDto
    {
        public int id_Consulta { get; set; }

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly? Data { get; set; }

        public TimeOnly? Horario { get; set; }

        public string Status { get; set; }

        public string Especialidade { get; set; }
        public string Area { get; set; }

        public int PacienteId { get; set; }
        public int ProfessorId { get; set; }

        public string NomePaciente { get; set; }
        public string NomeProfessor { get; set; }
    }
}
