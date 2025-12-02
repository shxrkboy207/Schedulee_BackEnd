using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Schedulee.Models
{
    public class Postagem
    {
        [Key]
        public int Id { get; set; }

        public string? Titulo { get; set; }
        public string? Conteudo { get; set; }
        public string? ImagemUrl { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

        public int UsuarioId { get; set; }

        
        [JsonIgnore]
        public Usuario? Usuario { get; set; }

        public int? ContratadoPorId { get; set; }

        public double AvaliacaoMedia { get; set; }
    }
}
