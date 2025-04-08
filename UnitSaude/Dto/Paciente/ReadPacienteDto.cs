using UnitSaude.Models;

namespace UnitSaude.Dto.Paciente
{
    public class ReadPacienteDto
    {
        public int id { get; set; }
        public string cpf { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string telefone { get; set; }
        public DateTime? dataNascimento { get; set; }

    }
}