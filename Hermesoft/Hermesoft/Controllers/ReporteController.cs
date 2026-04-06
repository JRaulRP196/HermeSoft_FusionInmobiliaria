using HermeSoft_Fusion.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using HermeSoft_Fusion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize(Roles = "Ventas, Administrador")]
    public class ReporteController : Controller
    {
        private readonly VentaBusiness _ventaBusiness;
        private readonly LoteBusiness _loteBusiness;

        public ReporteController(VentaBusiness ventaBusiness, LoteBusiness loteBusiness)
        {
            _ventaBusiness = ventaBusiness;
            _loteBusiness = loteBusiness;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VentasMes(int idMapa, string mes)
        {
            if (string.IsNullOrWhiteSpace(mes))
            {
                return BadRequest("Debe seleccionar un mes.");
            }

            if (!DateTime.TryParseExact(mes, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out var mesSeleccionado))
            {
                return BadRequest("Formato de mes inv�lido.");
            }

            var ventas = await _ventaBusiness.ObtenerVentasPorMes(mesSeleccionado.Year, mesSeleccionado.Month);
            var lotesMapa = await _loteBusiness.ObtenerLotesMapa(idMapa);

            var codigosVendidos = new HashSet<string>(ventas.Select(v => v.CodLote), StringComparer.OrdinalIgnoreCase);
            var lotesVendidos = lotesMapa.Where(l => codigosVendidos.Contains(l.Codigo)).ToList();

            return Json(new
            {
                lotes = lotesVendidos,
                total = lotesVendidos.Count
            });
        }
        [HttpPost]
        public IActionResult GenerarPdf([FromBody] PdfRequestMap request)
        {
            var base64Data = request.ImagenBase64.Split(',')[1];
            var imageBytes = Convert.FromBase64String(base64Data);

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);

                    page.Content().Column(col =>
                    {
                        col.Spacing(15);

                        col.Item().Border(1).Padding(10).Row(row =>
                        {
                            row.RelativeItem().Text("REPORTE DE LOTES")
                                .FontSize(14)
                                .Bold();

                            row.ConstantItem(150).AlignRight().Text($"Fecha: {DateTime.Now:dd/MM/yyyy}")
                                .FontSize(10);
                        });

                        col.Item().Border(1).Padding(10).Row(row =>
                        {
                            row.RelativeItem().Border(1).Padding(10).Column(c =>
                            {
                                c.Item().Text("CONDOMINIO").Bold().FontSize(10);
                                c.Item().Text(request.Condominio).FontSize(9);
                            });

                            row.Spacing(10);

                            row.RelativeItem().Border(1).Padding(10).Column(c =>
                            {
                                c.Item().Text("TIPO DE REPORTE").Bold().FontSize(10);
                                c.Item().Text(request.TipoReporte).FontSize(9);
                            });
                        });

                        col.Item().Border(1).Padding(10).Extend().Element(e =>
                        {
                            e.Image(imageBytes).FitWidth();
                        });

                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", "mapa.pdf");
        }

    }
}
