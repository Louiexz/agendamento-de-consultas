using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;



namespace UnitSaude.Models
{
    public abstract class Usuario
    {

        public int Id_Usuario { get; set; }
        public String cpf { get; set; }
        public String nome { get; set; }
        public String email { get; set; }
        public String senhaHash { get; private set; }
        public String telefone { get; set; }
        public DateTime dataCadastro { get; set; } = DateTime.Now;
        public DateTime? dataNascimento { get; set; }
        public String TipoUsuario { get; set; }
        public bool ativo { get; set; } = true;




    }
}
