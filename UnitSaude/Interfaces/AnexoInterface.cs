namespace UnitSaude.Interfaces
{
    using UnitSaude.Dto.Anexo;
    using UnitSaude.Models;

    public interface AnexoInterface
    {
        public Task<ResponseModel<object>> CadastrarAnexo(CreateAnexoDto anexo);
        public Task<ResponseModel<Anexo>> ListarAnexo(int AnexoId);
        public Task<ResponseModel<object>> GerenciarAnexo(UpdateAnexoDto anexo, int AnexoId);
        public Task<ResponseModel<object>> RemoverAnexo(int AnexoId);
    }
}