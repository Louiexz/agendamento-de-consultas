using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using UnitSaude.Models;

namespace UnitSaude.Data.Map
{
    public class ProfessoMap : IEntityTypeConfiguration<Professor>
    {
        public void Configure(EntityTypeBuilder<Professor> builder)
        {
            builder.HasKey(u => u.Id_Usuario);
            builder.Property(u => u.cpf).IsRequired().HasMaxLength(11);
            builder.Property(u => u.nome).IsRequired().HasMaxLength(100);
            builder.Property(u => u.email).IsRequired().HasMaxLength(255);
            builder.Property(u => u.senhaHash).IsRequired();
            builder.Property(u => u.telefone).HasMaxLength(20);
            builder.Property(u => u.dataCadastro).HasColumnType("date");
            builder.Property(u => u.dataNascimento).IsRequired(false).HasColumnType("date");
            builder.Property(u => u.TipoUsuario).HasMaxLength(50);
            builder.Property(u => u.ativo).HasDefaultValue(true);
            builder.Property(u => u.area).HasMaxLength(255);
            builder.Property(u => u.especialidade).HasMaxLength(255);
            builder.Property(u => u.codigoProfissional).HasMaxLength(255);


        }
    }
}
