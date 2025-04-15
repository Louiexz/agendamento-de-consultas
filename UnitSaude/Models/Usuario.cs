using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using UnitSaude.Utils;

namespace UnitSaude.Models
{
    public abstract class Usuario
    {

        public int Id_Usuario { get; set; }
        public string cpf { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public string senhaHash { get; set; }
        public string telefone { get; set; }

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly dataCadastro { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly? dataNascimento { get; set; }
        public string TipoUsuario { get; set; }
        public bool ativo { get; set; } = true;
    }
}
