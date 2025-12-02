using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedulee.DataBase;
using Schedulee.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

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

        public class UploadFotoRequest { public IFormFile File { get; set; } }

        private string HashSenha(string senha)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha ?? ""));
            return Convert.ToBase64String(bytes);
        }

        // ============================================================
        // CADASTRO
        // ============================================================
        [HttpPost("cadastro")]
        public async Task<IActionResult> Cadastrar([FromBody] Usuario usuario)
        {
            if (usuario == null || string.IsNullOrWhiteSpace(usuario.Email) || string.IsNullOrWhiteSpace(usuario.Senha))
                return BadRequest("Email e senha são obrigatórios.");

            var emailNorm = usuario.Email.Trim().ToLowerInvariant();

            if (await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == emailNorm))
                return BadRequest("E-mail já cadastrado.");

            usuario.Email = emailNorm;
            usuario.Senha = HashSenha(usuario.Senha.Trim());
            usuario.Bio ??= string.Empty;
            usuario.FotoPerfil ??= string.Empty;
            usuario.TipoConta ??= "Comum";
            usuario.Postagens ??= new List<Postagem>();

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { usuario.Id, usuario.Nome, usuario.Email });
        }

        // ============================================================
        // LOGIN
        // ============================================================
        public class LoginRequest { public string Email { get; set; } public string Senha { get; set; } }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Senha))
                return BadRequest("Email e senha são obrigatórios.");

            var emailNorm = req.Email.Trim().ToLowerInvariant();
            var senhaRaw = req.Senha.Trim();

            var existente = await _context.Usuarios
                .Include(u => u.Postagens)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == emailNorm);

            if (existente == null)
                return Unauthorized("Credenciais inválidas.");

            if (!string.Equals(existente.Senha, HashSenha(senhaRaw), StringComparison.Ordinal))
                return Unauthorized("Credenciais inválidas.");

            return Ok(new
            {
                existente.Id,
                existente.Nome,
                existente.Email,
                existente.FotoPerfil,
                existente.Bio,
                existente.IsMei,
                existente.TipoConta,
                existente.Postagens
            });
        }

        // ============================================================
        // GET PERFIL
        // ============================================================
        [HttpGet("{id}/perfil")]
        public async Task<IActionResult> GetPerfil(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Postagens)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            return Ok(new
            {
                usuario.Id,
                usuario.Nome,
                usuario.Email,
                usuario.FotoPerfil,
                usuario.Bio,
                usuario.IsMei,
                usuario.TipoConta,

                Postagens = usuario.Postagens.Select(p => new {
                    p.Id,
                    p.Titulo,
                    p.Conteudo,
                    p.CriadoEm,
                    p.ImagemUrl,
                    p.AvaliacaoMedia
                }).ToList()
            });
        }


        // ============================================================
        // EDITAR PERFIL  (AJUSTADO!)
        // ============================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarPerfil(int id, [FromBody] Usuario usuarioEditado)
        {
            try
            {
                if (usuarioEditado == null)
                    return BadRequest("Dados inválidos.");

                var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

                if (usuarioExistente == null)
                    return NotFound("Usuário não encontrado.");

                // Atualizar campos
                usuarioExistente.Nome = usuarioEditado.Nome ?? usuarioExistente.Nome;
                usuarioExistente.Telefone = usuarioEditado.Telefone ?? usuarioExistente.Telefone;
                usuarioExistente.Endereco = usuarioEditado.Endereco ?? usuarioExistente.Endereco;
                usuarioExistente.CPF = usuarioEditado.CPF ?? usuarioExistente.CPF;
                usuarioExistente.Bio = usuarioEditado.Bio ?? usuarioExistente.Bio;

                await _context.SaveChangesAsync();

                return Ok(usuarioExistente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        // ============================================================
        // UPLOAD FOTO DE PERFIL
        // ============================================================
        [HttpPost("{id}/foto")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFoto(int id, [FromForm] UploadFotoRequest request)
        {
            var arquivo = request?.File;
            if (arquivo == null || arquivo.Length == 0) return BadRequest("Arquivo inválido.");

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound("Usuário não encontrado.");

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
    }
}
