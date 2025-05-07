namespace UnitSaude.Models
{
    public class Professor : Usuario
    {
        public required string area { get; set; }
        public List<string> especialidades { get; set; } = new List<string>(); // Alterado para List<string>
        public required string codigoProfissional { get; set; }

        public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
      //  public ICollection<Prontuario> Prontuarios { get; set; }
     
    }
}
