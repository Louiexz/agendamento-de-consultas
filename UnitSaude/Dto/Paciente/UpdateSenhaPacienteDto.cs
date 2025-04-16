namespace UnitSaude.Dto.Paciente
{
    public class UpdateSenhaPacienteDto
    {
        public required string SenhaAtual { get; set; }
        public required string NovaSenha { get; set; }
    }
}