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

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);

                    page.Content().Column(col =>
                    {
                        col.Spacing(15);

                        col.Item().Border(1).Padding(10).Row(row =>
                        {
                            row.RelativeItem().Text("REPORTE DE PAGOS")
                                .FontSize(14)
                                .Bold();

                            row.ConstantItem(150).AlignRight().Text($"Fecha: {DateTime.Now:dd/MM/yyyy}")
                                .FontSize(10);
                        });

                        if(request.Desde != null && request.Hasta != null)
                        {
                            col.Item().Border(1).Padding(10).Row(row =>
                            {
                                row.RelativeItem().Border(1).Padding(10).Column(c =>
                                {
                                    c.Item().Text("DESDE").Bold().FontSize(10);
                                    c.Item().Text(request.Desde).FontSize(9);
                                });

                                row.Spacing(10);

                                row.RelativeItem().Border(1).Padding(10).Column(c =>
                                {
                                    c.Item().Text("HASTA").Bold().FontSize(10);
                                    c.Item().Text(request.Hasta).FontSize(9);
                                });
                            });
                        }

                        col.Item().Border(1).Padding(10).AlignCenter().AlignMiddle().Element(e =>
                        {
                            e.Image(imageBytes).FitWidth();
                        });

                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", "mapa.pdf");
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
