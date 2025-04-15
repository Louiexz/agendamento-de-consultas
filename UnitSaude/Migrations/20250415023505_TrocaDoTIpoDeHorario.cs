using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnitSaude.Migrations
{
    /// <inheritdoc />
    public partial class TrocaDoTIpoDeHorario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "Horario",
                table: "Consultas",
                type: "time without time zone",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "interval",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "Consultas",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "Consultas");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Horario",
                table: "Consultas",
                type: "interval",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone",
                oldNullable: true);
        }
    }
}
