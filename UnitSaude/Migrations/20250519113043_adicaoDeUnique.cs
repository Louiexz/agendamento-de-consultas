using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnitSaude.Migrations
{
    /// <inheritdoc />
    public partial class adicaoDeUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Professores_email",
                table: "Professores",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Professores_telefone",
                table: "Professores",
                column: "telefone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_email",
                table: "Pacientes",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_telefone",
                table: "Pacientes",
                column: "telefone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administradores_email",
                table: "Administradores",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Administradores_telefone",
                table: "Administradores",
                column: "telefone",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Professores_email",
                table: "Professores");

            migrationBuilder.DropIndex(
                name: "IX_Professores_telefone",
                table: "Professores");

            migrationBuilder.DropIndex(
                name: "IX_Pacientes_email",
                table: "Pacientes");

            migrationBuilder.DropIndex(
                name: "IX_Pacientes_telefone",
                table: "Pacientes");

            migrationBuilder.DropIndex(
                name: "IX_Administradores_email",
                table: "Administradores");

            migrationBuilder.DropIndex(
                name: "IX_Administradores_telefone",
                table: "Administradores");
        }
    }
}
