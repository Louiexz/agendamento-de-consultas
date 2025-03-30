namespace UnitSaude.Models
{
    public class Professor : Usuario
    {
        public string area { get; set; }
        public string especialidade { get; set; }
        public string codigoProfissional { get; set; }

        public ICollection<Consulta> Consultas { get; set; }
      //  public ICollection<Prontuario> Prontuarios { get; set; }
     
    }
}
