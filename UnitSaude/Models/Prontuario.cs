namespace UnitSaude.Models
{
    public class Prontuario
    {
        public int id_prontuario { get; set; }
        public string descricao { get; set; }
        public DateTime dataRegistro { get; set; } = DateTime.UtcNow;

        // Chaves estrangeiras
        public int pacienteId { get; set; }
        public int professorId { get; set; }

        // Navegação
        public Paciente paciente { get; set; }
        public Professor professor { get; set; }
       // public ICollection<Anexo> Anexos { get; set; }
    }
}
