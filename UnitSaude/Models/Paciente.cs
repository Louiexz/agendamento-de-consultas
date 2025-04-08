namespace UnitSaude.Models
{
    public class Paciente : Usuario
    {
        public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
        // public ICollection<Prontuario> Prontuarios { get; set; }
    }
}
