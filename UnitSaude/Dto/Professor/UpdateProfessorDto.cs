namespace UnitSaude.Dto.Professor
{
    public class UpdateProfessorDto
    {
        public string? email { get; set; }
        public string? senhaHash { get; private set; }
        public string? telefone { get; set; }
    }
}