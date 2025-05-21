using System.Text.Json.Serialization;
using UnitSaude.Utils;

namespace UnitSaude.Dto.Disponibilidade
{
    public class ReadDisponibilidadeDto
    {
        public int Id { get; set; }
        public DateOnly DataInicio { get; set; } // Data de início
        public DateOnly DataFim { get; set; } // Data de fim
        public TimeOnly HorarioInicio { get; set; }
        public TimeOnly HorarioFim { get; set; }
        public required string Area { get; set; }
        public required string Especialidade { get; set; }
    }
}
