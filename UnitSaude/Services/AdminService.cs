using UnitSaude.Data;

namespace UnitSaude.Services
{
    public class AdminService
    {
        private readonly ClinicaDbContext _context;
        public AdminService(ClinicaDbContext context)
        {
            _context = context;
        }
    }
}