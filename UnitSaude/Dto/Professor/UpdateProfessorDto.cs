namespace UnitSaude.Dto.Professor
{
    public class UpdateProfessorDto
    {
        public string cpf { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string telefone { get; set; }
        public DateTime? dataNascimento { get; set; }
        public string area { get; set; }
        public string especialidade { get; set; }
        public string codigoProfissional { get; set; }
    }
}