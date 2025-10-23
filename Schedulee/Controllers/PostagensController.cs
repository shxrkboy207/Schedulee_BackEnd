using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedulee.Models;

namespace Schedulee.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostagensController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostagensController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<IEnumerable<Postagem>> Get() =>
            await _context.Postagens.Include(p => p.Usuario).ToListAsync();

        [HttpPost]
        public async Task<IActionResult> Post(Postagem postagem)
        {
            _context.Postagens.Add(postagem);
            await _context.SaveChangesAsync();
            return Ok(postagem);
        }
    }
}
