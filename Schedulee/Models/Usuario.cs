using Schedulee.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Schedulee.Models{
public class Usuario
    {
    [Key]
    public int Id { get; set; }

    [Required]
    public string? Nome { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? Senha { get; set; }

    public string? Telefone { get; set; }

    public string? Endereco { get; set; }
    public string? CPF { get; set; }
    public string? FotoPerfil { get; set; }

    [JsonIgnore]
    public List<Postagem> Postagens { get; set; } = new();
    }
}