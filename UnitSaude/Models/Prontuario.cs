namespace UnitSaude.Models
{
    public class Prontuario
    {
        public int id_prontuario { get; set; }
        public required string descricao { get; set; }
        public DateTime dataRegistro { get; set; } = DateTime.UtcNow;

        // Chaves estrangeiras
        public int pacienteId { get; set; }
        public int professorId { get; set; }

        // Navegação
        public required Paciente paciente { get; set; }
        public required Professor professor { get; set; }
       // public ICollection<Anexo> Anexos { get; set; }
    }
}
