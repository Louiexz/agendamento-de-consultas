namespace UnitSaude.Interfaces
{
    using UnitSaude.Dto.Professor;
    using UnitSaude.Models;
    public interface ProfessorInterface
    {
        public Task<ResponseModel<object>> CadastrarProfessor(CreateProfessorDto professor);
        public Task<ResponseModel<ReadProfessorDto>> ListarProfessor(int ProfessorId);
        public Task<ResponseModel<List<ReadProfessorDto>>> ListarProfessoresPorEspecialidade(string especialidade);
        public Task<ResponseModel<object>> GerenciarProfessor(UpdateProfessorDto professor, int ProfessorId);
        public Task<ResponseModel<object>> RemoverProfessor(int ProfessorId);
    }
}