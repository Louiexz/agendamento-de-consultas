
namespace UnitSaude.Models
{
    public class Paciente : Usuario
    {
        public ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();

        public static Paciente FromObject(object v)
        {
            throw new NotImplementedException();
        }
        // public ICollection<Prontuario> Prontuarios { get; set; }
    }
}
