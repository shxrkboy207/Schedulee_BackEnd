using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Schedulee.Models
{
    public class MeiCadastro
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public string? Cnpj { get; set; }
        public string? Cep { get; set; }

        [Required]
        public string NomeFantasia { get; set; }

        public string? DocumentoFrente { get; set; }
        public string? DocumentoVerso { get; set; }

        public string? Bio { get; set; }

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}
