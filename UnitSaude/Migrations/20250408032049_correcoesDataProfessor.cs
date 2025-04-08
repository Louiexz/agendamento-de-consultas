using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnitSaude.Migrations
{
    /// <inheritdoc />
    public partial class correcoesDataProfessor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultas_Pacientes_PacienteId_Usuario",
                table: "Consultas");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultas_Professores_ProfessorId_Usuario",
                table: "Consultas");

            migrationBuilder.DropIndex(
                name: "IX_Consultas_PacienteId_Usuario",
                table: "Consultas");

            migrationBuilder.DropIndex(
                name: "IX_Consultas_ProfessorId_Usuario",
                table: "Consultas");

            migrationBuilder.DropColumn(
                name: "PacienteId_Usuario",
                table: "Consultas");

            migrationBuilder.DropColumn(
                name: "ProfessorId_Usuario",
                table: "Consultas");

            migrationBuilder.AlterColumn<DateTime>(
                name: "dataNascimento",
                table: "Professores",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "dataNascimento",
                table: "Professores",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PacienteId_Usuario",
                table: "Consultas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProfessorId_Usuario",
                table: "Consultas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_PacienteId_Usuario",
                table: "Consultas",
                column: "PacienteId_Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_ProfessorId_Usuario",
                table: "Consultas",
                column: "ProfessorId_Usuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultas_Pacientes_PacienteId_Usuario",
                table: "Consultas",
                column: "PacienteId_Usuario",
                principalTable: "Pacientes",
                principalColumn: "Id_Usuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultas_Professores_ProfessorId_Usuario",
                table: "Consultas",
                column: "ProfessorId_Usuario",
                principalTable: "Professores",
                principalColumn: "Id_Usuario");
        }
    }
}
