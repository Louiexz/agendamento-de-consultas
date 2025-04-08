using Newtonsoft.Json;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Consulta
{
    public class ReadConsultaDto
    {
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? Data { get; set; }
        public TimeSpan? Horario { get; set; }
        public string Status { get; set; }
        public string Especialidade { get; set; }
        public int PacienteId { get; set; }
        public int ProfessorId { get; set; }
    }
}
