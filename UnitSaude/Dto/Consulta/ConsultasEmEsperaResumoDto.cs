namespace UnitSaude.Dtos.Consulta
{
    public class ConsultasEmEsperaResumoDto
    {
        public string Area { get; set; }
        public string Especialidade { get; set; }
        public DateOnly Data { get; set; }
        public int TotalEmEspera { get; set; }
    }
}
