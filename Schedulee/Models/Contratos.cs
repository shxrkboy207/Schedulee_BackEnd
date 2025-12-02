
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Schedulee.Models
{
    public class Contrato
    {
        [Key]
        public int Id { get; set; }

        
        public int PostagemId { get; set; }
        [ForeignKey("PostagemId")]
        public Postagem Postagem { get; set; }

        public int ContratanteId { get; set; } 
        public int ContratadoId { get; set; } 

        public string Termo { get; set; } 
        public string Status { get; set; } 

        
        public string TempoServico { get; set; } 
        public decimal ValorNegociado { get; set; }

        
        public string AssinaturaContratante { get; set; }
        public string AssinaturaContratado { get; set; }

        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}