namespace UnitSaude.Interfaces
{
    using UnitSaude.Dto.Prontuario;
    using UnitSaude.Models;
    public interface ProntuarioInterface
    {
        public Task<ResponseModel<object>> CadastrarProntuario(CreateProntuarioDto prontuario);
        public Task<ResponseModel<Prontuario>> ListarProntuarioPorConsulta(int consultaId);
        public Task<ResponseModel<object>> GerenciarProntuario(CreateProntuarioDto prontuario, int prontuarioId);
    }
}