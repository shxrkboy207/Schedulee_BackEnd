
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedulee.DataBase;
using Schedulee.Models;

namespace Schedulee.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContratosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContratosController(AppDbContext context)
        {
            _context = context;
        }

        // POST api/contratos -> cria contrato
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContratoRequest req)
        {
            var postagem = await _context.Postagens.Include(p => p.Usuario).FirstOrDefaultAsync(p => p.Id == req.PostagemId);
            if (postagem == null) return NotFound("Postagem não encontrada.");

            if (postagem.UsuarioId == req.ContratanteId) return BadRequest("Não é possível contratar seu próprio serviço.");

            var contrato = new Contrato
            {
                PostagemId = req.PostagemId,
                ContratanteId = req.ContratanteId,
                ContratadoId = postagem.UsuarioId,
                Termo = req.Termo ?? DefaultTermo(),
                TempoServico = req.TempoServico,
                ValorNegociado = req.ValorNegociado,
                AssinaturaContratante = req.AssinaturaContratante, // contratante já assina no momento da criação
                Status = string.IsNullOrWhiteSpace(req.AssinaturaContratante) ? "Pendente" : "Assinado por Contratante"
            };

            _context.Contratos.Add(contrato);
            await _context.SaveChangesAsync();

            return Ok(contrato);
        }

        // PUT api/contratos/{id}/assinar -> usado para contratado assinar
        [HttpPut("{id}/assinar")]
        public async Task<IActionResult> Assinar(int id, [FromBody] AssinarRequest req)
        {
            var contrato = await _context.Contratos.FindAsync(id);
            if (contrato == null) return NotFound("Contrato não encontrado.");

            // verifica papel
            if (req.AssinanteId == contrato.ContratadoId)
            {
                contrato.AssinaturaContratado = req.Assinatura;
            }
            else if (req.AssinanteId == contrato.ContratanteId)
            {
                contrato.AssinaturaContratante = req.Assinatura;
            }
            else
            {
                return BadRequest("Usuário não tem permissão para assinar este contrato.");
            }

            // atualizar status 
            if (!string.IsNullOrWhiteSpace(contrato.AssinaturaContratante) && !string.IsNullOrWhiteSpace(contrato.AssinaturaContratado))
                contrato.Status = "Assinado por ambas as partes";
            else if (!string.IsNullOrWhiteSpace(contrato.AssinaturaContratante))
                contrato.Status = "Assinado por Contratante";
            else
                contrato.Status = "Pendente";

            await _context.SaveChangesAsync();
            return Ok(contrato);
        }

        // GET api/contratos/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var contrato = await _context.Contratos
                .Include(c => c.Postagem)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (contrato == null) return NotFound();
            return Ok(contrato);
        }

        private string DefaultTermo()
        {
            return "🛡️ Termo de Responsabilidade e Diretrizes de Segurança para Contratação de Serviços Autônomos\n\n" +
                   "Este documento estabelece as diretrizes de segurança e as responsabilidades mútuas entre o Contratante (o receptor dos serviços) e o Trabalhador Autônomo (o prestador dos serviços), intermediadas pelo sistema Schedulee.\n\n" +
                   "(...coloque aqui o texto completo que você descreveu...)";
        }
    }

    public class CreateContratoRequest
    {
        public int PostagemId { get; set; }
        public int ContratanteId { get; set; }
        public string TempoServico { get; set; }
        public decimal ValorNegociado { get; set; }
        public string AssinaturaContratante { get; set; } 
        public string Termo { get; set; } 
    }

    public class AssinarRequest
    {
        public int AssinanteId { get; set; }
        public string Assinatura { get; set; } 
}