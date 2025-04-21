using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UnitSaude.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoDaDisponibilidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Disponibilidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    DataFim = table.Column<DateOnly>(type: "date", nullable: false),
                    HorarioInicio = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    HorarioFim = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Area = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Especialidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disponibilidades", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Disponibilidades");
        }
    }
}
