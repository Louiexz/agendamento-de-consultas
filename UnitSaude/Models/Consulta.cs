namespace UnitSaude.Models
{
    public class Consulta
    {
        public int id_Consulta { get; set; }
        public DateTime? Data { get; set; }
        public TimeSpan? Horario { get; set; }
        public string Status { get; set; } 

        // Chaves estrangeiras
        public int PacienteId { get; set; }
        public int ProfessorId { get; set; }

        // Navegação
        public Paciente Paciente { get; set; }
        public Professor Professor { get; set; }

    }
}
