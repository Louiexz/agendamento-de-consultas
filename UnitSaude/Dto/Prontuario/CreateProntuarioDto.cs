namespace UnitSaude.Dto.Prontuario
{
    public class CreateProntuarioDto
    {
        public string descricao { get; set; }

        // Chaves estrangeiras
        public int pacienteId { get; set; }
        public int professorId { get; set; }
    }
}