using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnitSaude.Models;

namespace UnitSaude.Data.Map
{
    public class ConsultaMap : IEntityTypeConfiguration<Consulta>
    {
        public void Configure(EntityTypeBuilder<Consulta> builder)
        {
            builder.HasKey(u => u.id_Consulta);
            builder.Property(u => u.Data).IsRequired(false);
            builder.Property(u => u.Horario).IsRequired(false);
            builder.Property(u => u.Status).IsRequired().HasMaxLength(20);

            builder.HasOne(u => u.Paciente)
                   .WithMany()
                   .HasForeignKey(u => u.PacienteId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(u => u.Professor)
                .WithMany()
                .HasForeignKey(u => u.ProfessorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
