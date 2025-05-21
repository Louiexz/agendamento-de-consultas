namespace UnitSaude.Dtos.Consulta
{
    public class FiltroConsultaDto
    {
        public string? Status { get; set; } = string.Empty;
        public string? Area { get; set; }
        public string? Especialidade { get; set; }
        public DateOnly? Data { get; set; }
    }
}
