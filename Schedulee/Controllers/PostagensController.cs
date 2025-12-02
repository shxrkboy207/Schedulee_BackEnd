using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedulee.DataBase;
using Schedulee.Models;

namespace Schedulee.Controllers
{
    [ApiController]
    [Route("api/postagens")]
    public class PostagensController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostagensController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var lista = await _context.Postagens
                .Include(p => p.Usuario)
                .OrderByDescending(p => p.CriadoEm)
                .ToListAsync();

            var result = lista.Select(p => new {
                p.Id,
                p.Titulo,
                p.Conteudo,
                p.ImagemUrl,
                p.CriadoEm,
                p.AvaliacaoMedia,
                Usuario = new
                {
                    p.Usuario.Id,
                    p.Usuario.Nome,
                    p.Usuario.FotoPerfil
                }
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var postagem = await _context.Postagens
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (postagem == null) return NotFound();

            return Ok(new
            {
                postagem.Id,
                postagem.Titulo,
                postagem.Conteudo,
                postagem.ImagemUrl,
                postagem.CriadoEm,
                postagem.AvaliacaoMedia,
                Usuario = new { postagem.Usuario.Id, postagem.Usuario.Nome, postagem.Usuario.FotoPerfil }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Postagem post)
        {
            if (post == null || post.UsuarioId == 0) return BadRequest("Post inválido.");

            var usuario = await _context.Usuarios.FindAsync(post.UsuarioId);
            if (usuario == null) return NotFound("Usuário não encontrado.");
            if (!usuario.IsMei) return Forbid("Somente contas MEI podem criar postagens.");

            post.CriadoEm = DateTime.UtcNow;
            _context.Postagens.Add(post);
            await _context.SaveChangesAsync();

            var completa = await _context.Postagens
                .Include(x => x.Usuario)
                .FirstAsync(x => x.Id == post.Id);

            return CreatedAtAction(nameof(GetById), new { id = completa.Id }, new
            {
                completa.Id,
                completa.Titulo,
                completa.Conteudo,
                completa.ImagemUrl,
                completa.CriadoEm,
                completa.AvaliacaoMedia,
                Usuario = new { completa.Usuario.Id, completa.Usuario.Nome, completa.Usuario.FotoPerfil }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var postagem = await _context.Postagens.FindAsync(id);
            if (postagem == null) return NotFound();

            _context.Postagens.Remove(postagem);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
