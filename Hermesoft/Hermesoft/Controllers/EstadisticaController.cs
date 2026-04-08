using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Business.Usuarios;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Models.Usuarios;
using HermeSoft_Fusion.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize]
    public class EstadisticaController : Controller
    {

        private readonly EstadisticaBusiness _estadisticaBusiness;
        private readonly UsuarioBusiness _usuarioBusiness;

        public EstadisticaController(EstadisticaBusiness estadisticaBusiness, UsuarioBusiness usuarioBusiness)
        {
            _estadisticaBusiness = estadisticaBusiness;
            _usuarioBusiness = usuarioBusiness;
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Primas() 
        {
            Usuario usuario = await _usuarioBusiness.ObtenerConPrimas(User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value);
            return View(usuario.Ventas);
        }

        [HttpPost]
        public IActionResult GenerarPdf([FromBody] ReportePagoViewModel request)
        {
            var base64Data = request.GraficoBase64.Split(',')[1];
            var imageBytes = Convert.FromBase64String(base64Data);

            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/images/logos/logo_fusion_dark.png");
            var logoBytes = System.IO.File.Exists(logoPath)
                ? System.IO.File.ReadAllBytes(logoPath)
                : null;

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);

                    page.Header().Column(header =>
                    {
                        header.Item().Row(row =>
                        {
                            if (logoBytes != null)
                            {
                                row.ConstantItem(70).Height(45).Element(e =>
                                {
                                    e.Image(logoBytes).FitArea(); 
                                });
                            }

                            row.RelativeItem().AlignMiddle().Column(col =>
                            {
                                col.Item().Text("REPORTE DE PAGOS")
                                    .FontSize(20)
                                    .Bold()
                                    .FontColor("#1F3A5F");

                                col.Item().Text("Sistema Financiero")
                                    .FontSize(10)
                                    .FontColor(Colors.Grey.Darken1);
                            });

                            row.ConstantItem(150).AlignRight().Column(col =>
                            {
                                col.Item().Text("Fecha")
                                    .SemiBold()
                                    .FontSize(10);

                                col.Item().Text(DateTime.Now.ToString("dd/MM/yyyy"))
                                    .FontSize(12)
                                    .Bold();
                            });
                        });

                        header.Item()
                            .PaddingTop(5)
                            .LineHorizontal(2)
                            .LineColor("#1F3A5F");
                    });


                    page.Content().PaddingTop(15).Column(col =>
                    {
                        col.Spacing(15); 

                        if (request.Desde != null && request.Hasta != null)
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Background("#EEF2F7")
                                    .Padding(10) 
                                    .CornerRadius(8)
                                    .Column(c =>
                                    {
                                        c.Item().Text("Desde")
                                            .FontSize(10)
                                            .FontColor(Colors.Grey.Darken1);

                                        c.Item().Text(request.Desde)
                                            .FontSize(13)
                                            .Bold();
                                    });

                                row.Spacing(10);

                                row.RelativeItem().Background("#EEF2F7")
                                    .Padding(10) 
                                    .CornerRadius(8)
                                    .Column(c =>
                                    {
                                        c.Item().Text("Hasta")
                                            .FontSize(10)
                                            .FontColor(Colors.Grey.Darken1);

                                        c.Item().Text(request.Hasta)
                                            .FontSize(13)
                                            .Bold();
                                    });
                            });
                        }

                        col.Item().Background("#FFFFFF")
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .CornerRadius(10)
                            .Padding(15)
                            .Column(c =>
                            {
                                c.Item().Text("Visualización de Pagos")
                                    .FontSize(13)
                                    .Bold()
                                    .FontColor("#1F3A5F");

                                c.Item().PaddingTop(5)
                                    .LineHorizontal(1)
                                    .LineColor(Colors.Grey.Lighten2);

                                c.Item().PaddingTop(10)
                                    .AlignCenter()
                                    .MaxHeight(300)
                                    .Element(e =>
                                    {
                                        e.Image(imageBytes).FitWidth(); 
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

            return File(pdfBytes, "application/pdf", "reporte_pagos.pdf");
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

    }
}
