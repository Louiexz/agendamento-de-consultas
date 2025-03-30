using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UnitSaude.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administrador",
                columns: table => new
                {
                    Id_Usuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    senhaHash = table.Column<string>(type: "text", nullable: false),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    dataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    dataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TipoUsuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrador", x => x.Id_Usuario);
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    Id_Usuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    senhaHash = table.Column<string>(type: "text", nullable: false),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    dataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    dataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TipoUsuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.Id_Usuario);
                });

            migrationBuilder.CreateTable(
                name: "Professores",
                columns: table => new
                {
                    Id_Usuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    area = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    especialidade = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    codigoProfissional = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    senhaHash = table.Column<string>(type: "text", nullable: false),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    dataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    dataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TipoUsuario = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professores", x => x.Id_Usuario);
                });

            migrationBuilder.CreateTable(
                name: "Consultas",
                columns: table => new
                {
                    id_Consulta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Horario = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    PacienteId = table.Column<int>(type: "integer", nullable: false),
                    ProfessorId = table.Column<int>(type: "integer", nullable: false),
                    PacienteId_Usuario = table.Column<int>(type: "integer", nullable: true),
                    ProfessorId_Usuario = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultas", x => x.id_Consulta);
                    table.ForeignKey(
                        name: "FK_Consultas_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id_Usuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultas_Pacientes_PacienteId_Usuario",
                        column: x => x.PacienteId_Usuario,
                        principalTable: "Pacientes",
                        principalColumn: "Id_Usuario");
                    table.ForeignKey(
                        name: "FK_Consultas_Professores_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "Professores",
                        principalColumn: "Id_Usuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultas_Professores_ProfessorId_Usuario",
                        column: x => x.ProfessorId_Usuario,
                        principalTable: "Professores",
                        principalColumn: "Id_Usuario");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_PacienteId",
                table: "Consultas",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_PacienteId_Usuario",
                table: "Consultas",
                column: "PacienteId_Usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_ProfessorId",
                table: "Consultas",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_ProfessorId_Usuario",
                table: "Consultas",
                column: "ProfessorId_Usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrador");

            migrationBuilder.DropTable(
                name: "Consultas");

            migrationBuilder.DropTable(
                name: "Pacientes");

            migrationBuilder.DropTable(
                name: "Professores");
        }
    }
}
