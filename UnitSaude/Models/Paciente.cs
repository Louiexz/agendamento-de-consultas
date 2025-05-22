
namespace UnitSaude.Models
{
    public class Paciente : Usuario
    {
        public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();
        // public ICollection<Prontuario> Prontuarios { get; set; }

        public required string cep { get; set; }
        public required string estado { get; set; }
        public required string cidade { get; set; }
    }
}
