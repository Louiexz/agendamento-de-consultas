using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnitSaude.Models;

namespace UnitSaude.Data.Map
{
    public class DisponibilidadeMap : IEntityTypeConfiguration<Disponibilidade>
    {
        public void Configure(EntityTypeBuilder<Disponibilidade> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.DataInicio)
                .IsRequired()
                .HasColumnType("date");


            builder.Property(d => d.DataFim)
                .IsRequired()
                .HasColumnType("date");

            builder.Property(d => d.HorarioInicio)
                .IsRequired();

            builder.Property(d => d.HorarioFim)
                .IsRequired();

            builder.Property(d => d.Area)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Especialidade)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Ativo)
                .HasDefaultValue(true);
        }
    }
}
