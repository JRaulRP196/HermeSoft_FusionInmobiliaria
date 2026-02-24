using HermeSoft_Fusion.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize(Roles = "Ventas, Administrador")]
    public class CalculosController : Controller
    {

        private CalculosBusiness _calculosBusiness;

        public CalculosController(CalculosBusiness calculosBusiness)
        {
            _calculosBusiness = calculosBusiness;
        }

        [HttpGet]
        public async Task<IActionResult> CalcularPrima(string codigoLote, decimal porcentajePrima, DateTime fechaFinal)
        {
            try
            {
                var desglose = await _calculosBusiness.CalcularPrima(codigoLote, porcentajePrima, fechaFinal);
                TempData["DesglosePrima"] = JsonConvert.SerializeObject(desglose);
                return RedirectToAction("Prima", "Ventas", new {lote = codigoLote});

            }catch (Exception ex)
            {
                TempData["ErrorPrima"] = ex.ToString();
                return RedirectToAction("Prima", "Ventas");
            }
        }
        [HttpGet]
        public IActionResult ObtenerTimbre()
        {
            var timbre = _calculosBusiness.ObtenerTimbreFiscal();
            return Json(timbre);
        }


    }
}
