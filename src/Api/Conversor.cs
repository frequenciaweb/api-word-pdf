using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using System.IO.Packaging;
using System.Text;
using System.Xml.Linq;

namespace Api
{
    public class Conversor
    {
        public string ConverterWordParaHtml(string arquivo, Encoding encoding)
        {
            var source = Package.Open(arquivo);
            var document = WordprocessingDocument.Open(source);
            var settings = new HtmlConverterSettings();
            XElement html = HtmlConverter.ConvertToHtml(document, settings);
            return html.ToString();
        }

        private static string ConverteEncode1252(string value)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding wind1252 = Encoding.GetEncoding(1252);
            return wind1252.GetString(wind1252.GetBytes(value));
        }

        public byte[] ConverterHtmlParaPDF(string html)
        {
            var pasta = "arquivos-gerados";
            var diretorio = Path.Combine(Directory.GetCurrentDirectory(), pasta);
            Directory.CreateDirectory(diretorio);
            string nomeHtml = Guid.NewGuid().ToString() + ".html";
            string nomePDF = nomeHtml.Replace(".html", ".pdf");

            var writer = File.CreateText(diretorio+"\\"+nomeHtml);
            writer.WriteLine(html);
            writer.Dispose();
            var arquivoHtml = File.ReadAllText(diretorio + "\\" + nomeHtml);

            byte[]? res = null;
            using (MemoryStream ms = new MemoryStream())
            {
                var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(ConverteEncode1252(arquivoHtml), PdfSharp.PageSize.A4);
                pdf.Save(ms);
                res = ms.ToArray();
            }
            return res;
        }
    }
}
