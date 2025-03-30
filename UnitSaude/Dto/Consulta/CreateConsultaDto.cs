namespace UnitSaude.Dto.Consulta
{
    public class CreateConsultaDto
    {
        public DateTime? Data { get; set; }
        public TimeSpan? Horario { get; set; }
        public string Especialidade { get; set; }
    }
}