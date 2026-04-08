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

            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images/logos/logo_fusion_dark.png");
            var logoBytes = System.IO.File.Exists(logoPath)
                ? System.IO.File.ReadAllBytes(logoPath)
                : null;

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor("#F8F9FB"); 

                    page.Header().Column(header =>
                    {
                        header.Item().Row(row =>
                        {
                            if (logoBytes != null)
                            {
                                row.ConstantItem(80).Image(logoBytes);
                            }

                            row.RelativeItem().AlignMiddle().Column(col =>
                            {
                                col.Item().Text("REPORTE DE LOTES")
                                    .FontSize(18)
                                    .Bold()
                                    .FontColor("#1F3A5F");

                                col.Item().Text("Sistema de Reportes")
                                    .FontSize(10)
                                    .FontColor(Colors.Grey.Darken1);
                            });

                            row.ConstantItem(150).AlignRight().Column(col =>
                            {
                                col.Item().Text("Fecha")
                                    .SemiBold()
                                    .FontSize(10);

                                col.Item().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                                    .Bold()
                                    .FontSize(12);
                            });
                        });

                        header.Item().PaddingTop(5)
                            .LineHorizontal(2)
                            .LineColor("#1F3A5F");
                    });

                    page.Content().PaddingTop(15).Column(col =>
                    {
                        col.Spacing(20);

                        col.Item().Background("#FFFFFF")
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .CornerRadius(10)
                            .Padding(15)
                            .Column(c =>
                            {
                                c.Item().Text("Información General")
                                    .FontSize(13)
                                    .Bold()
                                    .FontColor("#1F3A5F");

                                c.Item().LineHorizontal(1)
                                    .LineColor(Colors.Grey.Lighten2);

                                c.Item().PaddingTop(5)
                                    .Text($"Condominio: {request.Condominio}")
                                    .FontSize(11);

                                c.Item().Text($"Tipo de reporte: {request.TipoReporte}")
                                    .FontSize(11);
                            });

                        col.Item().Background("#FFFFFF")
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .CornerRadius(10)
                            .Padding(20)
                            .Column(c =>
                            {
                                c.Item().Text("Mapa del Proyecto")
                                    .FontSize(13)
                                    .Bold()
                                    .FontColor("#1F3A5F");

                                c.Item().LineHorizontal(1)
                                    .LineColor(Colors.Grey.Lighten2);

                                c.Item().PaddingTop(15)
                                    .AlignCenter()
                                    .Height(350) 
                                    .Element(e =>
                                    {
                                        e.Image(imageBytes).FitArea();
                                    });
                            });
                    });

                    page.Footer().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().Text("Fusion Inmobiliaria")
                            .FontSize(9)
                            .FontColor(Colors.Grey.Darken1);

                        row.ConstantItem(150).AlignRight().Text(text =>
                        {
                            text.Span("Página ");
                            text.CurrentPageNumber();
                            text.Span(" de ");
                            text.TotalPages();
                        });
                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", "mapa.pdf");
        }

    }
}
