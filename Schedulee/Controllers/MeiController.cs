using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Schedulee.DataBase;
using Schedulee.Models;

namespace Schedulee.Controllers
{
    [ApiController]
    [Route("api/meicadastro")]
    public class MeiCadastroController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MeiCadastroController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> GetByUser(int usuarioId)
        {
            var mei = await _context.MeiCadastros
                .Include(m => m.Usuario)
                .FirstOrDefaultAsync(m => m.UsuarioId == usuarioId);

            if (mei == null) return NotFound();

            return Ok(new
            {
                mei.Id,
                mei.NomeFantasia,
                mei.Cnpj,
                mei.Cep,
                mei.Bio,
                Usuario = new { mei.Usuario.Id, mei.Usuario.Nome }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MeiCadastro mei)
        {
            if (mei == null || mei.UsuarioId == 0) return BadRequest("Dados inválidos.");
            if (string.IsNullOrWhiteSpace(mei.NomeFantasia)) return BadRequest("Nome Fantasia é obrigatório.");

            var usuario = await _context.Usuarios.FindAsync(mei.UsuarioId);
            if (usuario == null) return NotFound("Usuário não encontrado.");

            var existente = await _context.MeiCadastros.FirstOrDefaultAsync(m => m.UsuarioId == mei.UsuarioId);
            if (existente != null) return BadRequest("Cadastro MEI já existente.");

            _context.MeiCadastros.Add(mei);

            usuario.IsMei = true;
            usuario.TipoConta = "Empreendedor";
            if (!string.IsNullOrWhiteSpace(mei.Bio)) usuario.Bio = mei.Bio;

            await _context.SaveChangesAsync();

            return Ok(new { message = "MEI ativado com sucesso!", usuario = new { usuario.Id, usuario.Nome, usuario.FotoPerfil } });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] MeiCadastro mei)
        {
            var existing = await _context.MeiCadastros.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Cnpj = mei.Cnpj;
            existing.Cep = mei.Cep;
            existing.NomeFantasia = mei.NomeFantasia;
            existing.DocumentoFrente = mei.DocumentoFrente;
            existing.DocumentoVerso = mei.DocumentoVerso;
            existing.Bio = mei.Bio;

            var usuario = await _context.Usuarios.FindAsync(existing.UsuarioId);
            if (usuario != null && !string.IsNullOrWhiteSpace(mei.Bio)) usuario.Bio = mei.Bio;

            await _context.SaveChangesAsync();
            return Ok(new { existing.Id, existing.NomeFantasia });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var mei = await _context.MeiCadastros.FindAsync(id);
            if (mei == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(mei.UsuarioId);
            if (usuario != null)
            {
                usuario.IsMei = false;
                usuario.TipoConta = "Comum";
            }

            _context.MeiCadastros.Remove(mei);
            await _context.SaveChangesAsync();

            return Ok("MEI removido");
        }
    }
}
