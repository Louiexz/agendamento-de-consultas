using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UnitSaude.Models;

namespace UnitSaude.Data
{
    public class ClinicaDbContext : DbContext
    {
        public ClinicaDbContext(DbContextOptions<ClinicaDbContext> options)
            : base(options)
        {
        }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Professor> Professores { get; set; }
        public DbSet<Consulta> Consultas { get; set; }
        public DbSet<Disponibilidade> Disponibilidades { get; set; }


        //public DbSet<Prontuario> Prontuarios { get; set; }
        // public DbSet<Anexo> Anexos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
