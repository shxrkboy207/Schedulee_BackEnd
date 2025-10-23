using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedulee.Models;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Schedulee.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UsuariosController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost("cadastro")]
        public async Task<IActionResult> Cadastrar([FromBody] Usuario usuario)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
                return BadRequest("E-mail já cadastrado.");

            usuario.Senha = HashSenha(usuario.Senha);
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(usuario);
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario usuario)
        {
            var existente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == usuario.Email);
            if (existente == null || existente.Senha != HashSenha(usuario.Senha))
                return Unauthorized("E-mail ou senha incorretos.");

            return Ok(existente);
        }

        [HttpPost("{id}/foto")]
        public async Task<IActionResult> UploadFoto(int id, IFormFile arquivo)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            var caminho = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
            Directory.CreateDirectory(caminho);

            var nomeArquivo = $"{Guid.NewGuid()}_{arquivo.FileName}";
            var caminhoArquivo = Path.Combine(caminho, nomeArquivo);

            using var stream = new FileStream(caminhoArquivo, FileMode.Create);
            await arquivo.CopyToAsync(stream);

            usuario.FotoPerfil = $"/uploads/{nomeArquivo}";
            await _context.SaveChangesAsync();

            return Ok(usuario.FotoPerfil);
        }

        private string HashSenha(string senha)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha));
            return Convert.ToBase64String(bytes);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Usuario usuario)
        {
            var existente = await _context.Usuarios
                .Include(u => u.Postagens)
                .FirstOrDefaultAsync(u => u.Email == usuario.Email);

            if (existente == null || existente.Senha != HashSenha(usuario.Senha))
                return Unauthorized("E-mail ou senha incorretos.");

            return Ok(new
            {
                existente.Id,
                existente.Nome,
                existente.Email,
                existente.FotoPerfil,
                existente.Postagens
            });
        }

        [HttpGet("{id}/perfil")]
        public async Task<IActionResult> GetPerfil(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Postagens)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            return Ok(usuario);
        }

    }
}
