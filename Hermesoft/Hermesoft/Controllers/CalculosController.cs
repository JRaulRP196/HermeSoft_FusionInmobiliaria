using HermeSoft_Fusion.Business;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HermeSoft_Fusion.Controllers
{
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
                _calculosBusiness.ObtenerTimbreFiscal(); //Esta función es solo para verificar que se trae bien el timbre fiscal del appsettings
                var desglose = await _calculosBusiness.CalcularPrima(codigoLote, porcentajePrima, fechaFinal);

                TempData["DesglosePrima"] = JsonConvert.SerializeObject(desglose);
                TempData["lote"] = codigoLote;
                return RedirectToAction("Registro", "Ventas");

            }catch (Exception ex)
            {
                TempData["ErrorPrima"] = ex.ToString();
                return RedirectToAction("Registro", "Ventas");
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
