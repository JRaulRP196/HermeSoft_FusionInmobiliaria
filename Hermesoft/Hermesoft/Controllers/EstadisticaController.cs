using HermeSoft_Fusion.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EstadisticaController : Controller
    {

        private readonly EstadisticaBusiness _estadisticaBusiness;

        public EstadisticaController(EstadisticaBusiness estadisticaBusiness)
        {
            _estadisticaBusiness = estadisticaBusiness;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Primas()
        {
            return View();
        }

        #region js

        [HttpGet]
        public async Task<IActionResult> PagosPorCondominio(string condominio, DateTime fechaInicio, DateTime fechaFinal)
        {
            try
            {
                var resultado = await _estadisticaBusiness.PagosPorCondominio(condominio, fechaInicio, fechaFinal);
                return Json(resultado);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    message = e.Message
                });
            }
        }

        #endregion


        [HttpPost]
        public async Task<IActionResult> DescargarPdf([FromBody] dynamic request)
        {
            try
            {
                DateTime fechaInicio = request.fechaInicio;
                DateTime fechaFinal = request.fechaFinal;
                var condominios = request.condominios;

                var datos = await _estadisticaBusiness.PagosTodos(condominios, fechaInicio, fechaFinal);

                // ❌ SIN DATOS
                if (datos == null || !datos.Any())
                {
                    return BadRequest("No hay datos para generar el reporte.");
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    var writer = new PdfWriter(ms);
                    var pdf = new PdfDocument(writer);
                    var document = new Document(pdf);


                    var boldFont = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);

                    document.Add(new Paragraph("Reporte de Motricidad de Pagos de Prima")
                        .SetFont(boldFont)
                        .SetFontSize(18));


                    document.Add(new Paragraph($"Fecha de generación: {DateTime.Now}"));

                    document.Add(new Paragraph("\n"));

               
                    var table = new Table(4);

                    table.AddHeaderCell("Condominio");
                    table.AddHeaderCell("Pagados");
                    table.AddHeaderCell("Pendientes");
                    table.AddHeaderCell("Atrasados");

                    foreach (var item in datos)
                    {
                        table.AddCell(item.Condominio ?? "N/A");
                        table.AddCell(item.Pagados.ToString());
                        table.AddCell(item.Pendientes.ToString());
                        table.AddCell(item.Atrasados.ToString());
                    }

                    document.Add(table);
                    document.Close();

                    return File(ms.ToArray(), "application/pdf", "ReportePagos.pdf");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}