namespace UnitSaude.Models
{
    public class Professor : Usuario
    {
        public required string area { get; set; }
        public required string especialidade { get; set; }
        public required string codigoProfissional { get; set; }

        public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
      //  public ICollection<Prontuario> Prontuarios { get; set; }
     
    }
}
