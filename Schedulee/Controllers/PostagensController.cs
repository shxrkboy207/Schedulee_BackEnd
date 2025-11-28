using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedulee.DataBase;
using Schedulee.Models;

namespace Schedulee.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostagensController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostagensController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CriarPost([FromBody] Postagem postagem)
        {
            var usuario = await _context.Usuarios.FindAsync(postagem.UsuarioId);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            _context.Postagens.Add(postagem);
            await _context.SaveChangesAsync();

            return Ok(postagem);
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> GetPostsUsuario(int usuarioId)
        {
            var posts = await _context.Postagens
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.DataPostagem)
                .ToListAsync();

            return Ok(posts);
        }
    }
}
