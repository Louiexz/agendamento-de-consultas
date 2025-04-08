namespace UnitSaude.Dto.Admin
{
    public class ReadAdminDto
    {
        public string id { get; set; }
        public string cpf { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        //public string senhaHash { get; private set; } = "";
        public string telefone { get; set; }
        public DateTime? dataNascimento { get; set; }
    }
}