using Microsoft.AspNetCore.Mvc;

namespace Schedulee.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost("imagem")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImagem([FromForm] IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0) return BadRequest("Arquivo inválido.");

            var root = _env.WebRootPath;
            if (string.IsNullOrEmpty(root)) root = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            Directory.CreateDirectory(root);

            var caminho = Path.Combine(root, "uploads");
            Directory.CreateDirectory(caminho);

            var ext = Path.GetExtension(arquivo.FileName);
            var nome = $"{Guid.NewGuid()}{ext}";
            var caminhoArquivo = Path.Combine(caminho, nome);

            using var stream = new FileStream(caminhoArquivo, FileMode.Create);
            await arquivo.CopyToAsync(stream);

            var url = $"{Request.Scheme}://{Request.Host}/uploads/{nome}";
            return Ok(url);
        }
    }
}
