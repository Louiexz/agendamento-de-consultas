namespace UnitSaude.Interfaces
{
    using UnitSaude.Dto.Paciente;
    using UnitSaude.Dto.Professor;
    using UnitSaude.Models;
    public interface ProfessorInterface
    {
        public Task<ResponseModel<ReadProfessorDto>> CadastrarProfessor(CreateProfessorDto professorDto);
        public Task<ResponseModel<List<ReadProfessorDto>>> ListarProfessores();
        public Task<ResponseModel<ReadProfessorDto>> ListarProfessor(int professorId);
        public Task<ResponseModel<ReadProfessorDto>> GerenciarProfessor(UpdateProfessorDto professorDto, int professorId);
        public Task<ResponseModel<string>> AlterarSenhaProfessor(int professorId, UpdateSenhaProfessorDto dto);
        public Task<ResponseModel<Professor>> RemoverProfessor(int ProfessorId);

        public Task<bool> ResetarSenhaProfessor(int id, string novaSenhaHash);
    }
}