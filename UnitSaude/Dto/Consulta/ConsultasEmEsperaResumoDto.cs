namespace UnitSaude.Dtos.Consulta
{
    public class ConsultasEmEsperaResumoDto
    {
        public required string Area { get; set; }
        public required string Especialidade { get; set; }
        public DateOnly Data { get; set; }
        public int TotalEmEspera { get; set; }
    }
}
