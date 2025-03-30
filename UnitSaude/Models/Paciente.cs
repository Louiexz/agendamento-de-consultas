namespace UnitSaude.Models
{
    public class Paciente : Usuario
    {
        public ICollection<Consulta> Consultas { get; set; }
       // public ICollection<Prontuario> Prontuarios { get; set; }
    }
}
