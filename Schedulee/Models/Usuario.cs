using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Schedulee.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; } = null!;

        [Required]
        public string Email { get; set; } = null!;

        [Required]
        public string Senha { get; set; } = null!;

       
        public string? Telefone { get; set; }
        public string? Endereco { get; set; }
        public string? CPF { get; set; }
        public string? FotoPerfil { get; set; }

        public bool IsMei { get; set; } = false;
        public string TipoConta { get; set; } = "Comum";

        public string? Bio { get; set; }

        
        [JsonIgnore]
        public List<Postagem> Postagens { get; set; } = new List<Postagem>();
    }
}
