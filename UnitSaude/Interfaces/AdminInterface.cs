namespace UnitSaude.Interfaces
{
    using UnitSaude.Dto.Admin;
    using UnitSaude.Models;

    public interface AdminInterface
    {
        public Task<ResponseModel<object>> CadastrarAdmin(CreateAdminDto admin);
        public Task<ResponseModel<ReadAdminDto>> ListarAdmin();
        public Task<ResponseModel<object>> GerenciarAdmin(UpdateAdminDto admin, int AdminId);
    }
}