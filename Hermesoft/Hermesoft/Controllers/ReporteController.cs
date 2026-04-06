using HermeSoft_Fusion.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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
                return BadRequest("Formato de mes inválido.");
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
    }
}
