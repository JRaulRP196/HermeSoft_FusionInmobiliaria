using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Models.Banco;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
        public async Task<IActionResult> CalcularPrima(
                                        string codigoLote,
                                        decimal porcentajePrima,
                                        DateTime fechaFinal,
                                        decimal? porcentajeDescuento )
        {
            try
            {
                var desgloseSinDescuento = await _calculosBusiness.CalcularPrima(
                    codigoLote,
                    porcentajePrima,
                    fechaFinal);

                IEnumerable<DesglosesPrimas> desgloseConDescuento = new List<DesglosesPrimas>();

                if (porcentajeDescuento.HasValue && porcentajeDescuento.Value > 0)
                {
                    desgloseConDescuento = await _calculosBusiness.CalcularPrima(
                        codigoLote,
                        porcentajePrima,
                        fechaFinal,
                        porcentajeDescuento);
                }

                TempData["DesglosePrimaSinDescuento"] = JsonConvert.SerializeObject(desgloseSinDescuento);
                TempData["DesglosePrimaConDescuento"] = JsonConvert.SerializeObject(desgloseConDescuento);
                TempData["PorcentajeDescuento"] = (porcentajeDescuento ?? 0m).ToString();
                TempData["Porcentaje"] = porcentajePrima.ToString();
                TempData["FechaFinal"] = fechaFinal.ToString("yyyy-MM-dd");

                return RedirectToAction("Prima", "Ventas", new { lote = codigoLote });
            }
            catch (Exception ex)
            {
                TempData["ErrorPrima"] = ex.ToString();
                return RedirectToAction("Prima", "Ventas", new { lote = codigoLote });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CalcularCuotasBancariaJS(int idEscenario, string codigoLote)
        {
            try
            {
                return Json(await _calculosBusiness.CalcularCuotasBancaria(idEscenario, codigoLote));
            }
            catch (Exception ex)
            {
                TempData["ErrorCalculoCuotas"] = ex.ToString();
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> CalcularGastoFormalizacionJS(decimal seguroVida, decimal seguroDesempleo, decimal honorarioAbogados,
            decimal comisionBancaria, string codLote)
        {
            try
            {
                return Json(await _calculosBusiness.CalcularGastoFormalizacion(seguroVida,seguroDesempleo,honorarioAbogados,comisionBancaria,codLote));
            }
            catch (Exception ex)
            {
                TempData["ErrorCalculoFormalizacion"] = ex.ToString();
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> CalcularIngresoNetoFamiliarJS(int idBanco, int idEndeudamiento, decimal cuotaMensual)
        {
            try
            {
                return Json(await _calculosBusiness.CalcularIngresoNetoFamiliar(idBanco, idEndeudamiento, cuotaMensual));
            }catch(Exception ex)
            {
                TempData["ErrorCalculoIngresoNetoFamiliar"] = ex.ToString();
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCambioDelDolarJS()
        {
            try
            {
                return Json(await _calculosBusiness.ObtenerCambioDelDolar());
            }
            catch(Exception ex)
            {
                TempData["ErrorCambioDolar"] = ex.ToString();
                return NotFound();
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
