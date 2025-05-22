namespace UnitSaude.Dto.Consulta
{
    public class UpdateConsultaDto
    {
        public DateOnly? Data { get; set; }
        public TimeOnly? Horario { get; set; }
        public string? Anamnese { get; set; }
        public int ProfessorId { get; set; }
    }
}