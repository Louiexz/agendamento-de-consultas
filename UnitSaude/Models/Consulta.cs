namespace UnitSaude.Models
{
    public class Consulta
    {
        public int id_Consulta { get; set; }
        public DateOnly? Data { get; set; }
        public TimeOnly? Horario { get; set; }
        public string Status { get; set; }
        public string Area { get; set; }
        public string Especialidade { get; set; }


        // Chaves estrangeiras
        public int PacienteId { get; set; }
        public int ProfessorId { get; set; }

        // Navegação
        public Paciente Paciente { get; set; }
        public Professor Professor { get; set; }

    }
}
