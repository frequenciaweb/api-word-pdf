using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversorController : ControllerBase
    {


        private readonly ILogger<ConversorController> _logger;

        public ConversorController(ILogger<ConversorController> logger)
        {
            _logger = logger;
        }

        private IActionResult Download(byte[] arquivo, string tipoConteudo, string nomeArquivo)
        {
            return File(new MemoryStream(arquivo), tipoConteudo, nomeArquivo);
        }

        [HttpPost(Name = "word-to-pdf"), DisableRequestSizeLimit]
        public IActionResult WordToPDF()
        {
            try
            {
                var anexo = Request.Form.Files[0];
                if (!anexo.FileName.ToUpper().Contains(".DOCX"))
                {
                    return BadRequest("Arquivo inválido");
                }
                var pasta = "uploads";
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), pasta);
                Directory.CreateDirectory(uploads);
                if (anexo.Length > 0)
                {
                    var nome = Guid.NewGuid().ToString()+".docx";
                    string arquivoUpload = uploads + "\\" + nome;

                    using (FileStream stream = new FileStream(arquivoUpload, FileMode.Create))
                    {
                        anexo.CopyTo(stream);
                    }

                    Conversor conversor = new Conversor();
                    string arquivohtml = conversor.ConverterWordParaHtml(arquivoUpload, Encoding.UTF8);
                    byte[] arquivoPDF = conversor.ConverterHtmlParaPDF(arquivohtml);

                    return Download(arquivoPDF, "application/pdf", ContentDispositionHeaderValue.Parse(anexo.ContentDisposition).FileName.Trim('"'));
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}