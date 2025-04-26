using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnitSaude.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBanco : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Professores_cpf",
                table: "Professores",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_cpf",
                table: "Pacientes",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administradores_cpf",
                table: "Administradores",
                column: "cpf",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Professores_cpf",
                table: "Professores");

            migrationBuilder.DropIndex(
                name: "IX_Pacientes_cpf",
                table: "Pacientes");

            migrationBuilder.DropIndex(
                name: "IX_Administradores_cpf",
                table: "Administradores");
        }
    }
}
