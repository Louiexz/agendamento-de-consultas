namespace UnitSaude.Dto.Professor
{
    public class ReadProfessorDto
    {
        public string id { get; set; }
        public string cpf { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string senhaHash { get; private set; } = "";
        public string telefone { get; set; }
        public DateTime? dataNascimento { get; set; }
        public string area { get; set; }
        public string especialidade { get; set; }
        public string codigoProfissional { get; set; }
    }
}