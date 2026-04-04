using HermeSoft_Fusion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReporteController : Controller
    {

        public IActionResult Index()
        {
            return View();
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
