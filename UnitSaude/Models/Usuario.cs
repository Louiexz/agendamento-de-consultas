using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using UnitSaude.Utils;

namespace UnitSaude.Models
{
    public abstract class Usuario
    {

        public int Id_Usuario { get; set; }
        public required string cpf { get; set; }
        public required string nome { get; set; }
        public required string email { get; set; }
        public required string senhaHash { get; set; }
        public required string telefone { get; set; }

        public DateOnly dataCadastro { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public DateOnly? dataNascimento { get; set; }
        public required string TipoUsuario { get; set; }
        public bool ativo { get; set; } = true;
    }
}
