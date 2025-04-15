namespace UnitSaude.Interfaces
{
    using UnitSaude.Dto.Admin;
    using UnitSaude.Models;

    public interface AdminInterface
    {
        public Task<ResponseModel<ReadAdminDto>> CadastrarAdmin(CreateAdminDto admin);
        public Task<ResponseModel<List<ReadAdminDto>>> ListarAdmins();
        public Task<ResponseModel<ReadAdminDto>> ListarAdmin(int AdminId);
        public Task<ResponseModel<ReadAdminDto>> GerenciarAdmin(UpdateAdminDto admin, int AdminId);
        public Task<ResponseModel<string>> AlterarSenhaAdmin(int adminId, UpdateSenhaAdminDto dto);
        public Task<ResponseModel<Administrador>> RemoverAdmin(int AdminId);

        public Task<bool> ResetarSenhaAdministrador(int id, string novaSenhaHash);
    }
}