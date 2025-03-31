using UnitSaude.Data;
using UnitSaude.Dto.Admin;
using UnitSaude.Interfaces;
using UnitSaude.Models;

namespace UnitSaude.Services
{
    public class AdminService : AdminInterface
    {
        private readonly ClinicaDbContext _context;
        public AdminService(ClinicaDbContext context)
        {
            _context = context;
        }

        public Task<ResponseModel<object>> CadastrarAdmin(CreateAdminDto admin)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<object>> GerenciarAdmin(UpdateAdminDto admin, int AdminId)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel<ReadAdminDto>> ListarAdmin()
        {
            throw new NotImplementedException();
        }
    }
}