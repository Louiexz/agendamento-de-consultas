namespace UnitSaude.Dto.Consulta
{
    public class UpdateConsultaDto
    {
        public DateTime? Data { get; set; }
        public TimeSpan? Horario { get; set; }
        public int ProfessorId { get; set; }
    }
}