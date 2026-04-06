using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Models.Banco;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HermeSoft_Fusion.Models.ViewModels;

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
        public async Task<IActionResult> CalcularPrima( string codigoLote, decimal porcentajePrima,
            DateTime fechaFinal, decimal? porcentajeDescuento)
        {
            var model = new PrimaViewModel
            {
                Lote = codigoLote,
                Porcentaje = porcentajePrima,
                PorcentajeDescuento = porcentajeDescuento ?? 0m,
                FechaCierre = fechaFinal
            };

            try
            {
                var desgloseSinDescuento = await _calculosBusiness.CalcularPrima(
                    codigoLote,
                    porcentajePrima,
                    fechaFinal);

                model.DesglosesSinDescuento = desgloseSinDescuento.ToList();

                if (porcentajeDescuento.HasValue && porcentajeDescuento.Value > 0)
                {
                    var desgloseConDescuento = await _calculosBusiness.CalcularPrima(
                        codigoLote,
                        porcentajePrima,
                        fechaFinal,
                        porcentajeDescuento);

                    model.DesglosesConDescuento = desgloseConDescuento.ToList();
                }

                return View("~/Views/Ventas/Prima.cshtml", model);
            }
            catch (Exception ex)
            {
                model.MensajeErrorPrima = "Ocurrió un error al calcular la prima.";
                return View("~/Views/Ventas/Prima.cshtml", model);
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

        [HttpGet] // Borrar
        public async Task<IActionResult> CalcularGastoFormalizacionJS2(decimal seguroVida, decimal seguroDesempleo, decimal honorarioAbogados,
            decimal comisionBancaria, string codLote)
        {
            try
            {
                return Json(await _calculosBusiness.CalcularGastoFormalizacion(seguroVida, seguroDesempleo, honorarioAbogados, comisionBancaria, codLote));
            }
            catch (Exception ex)
            {
                TempData["ErrorCalculoFormalizacion"] = ex.ToString();
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> CalcularGastoFormalizacionJS(decimal seguroVida, decimal seguroDesempleo, decimal honorarioAbogados,
    decimal comisionBancaria, string codLote)
        {
            try
            {
                var resultado = await _calculosBusiness.CalcularGastoFormalizacion(
                    seguroVida,
                    seguroDesempleo,
                    honorarioAbogados,
                    comisionBancaria,
                    codLote
                );

                return Json(resultado);
            }
            catch (Exception ex)
            {
                TempData["ErrorCalculoFormalizacion"] = ex.ToString();
                return BadRequest(ex.ToString());
            }
        }

        [HttpGet]
        public async Task<IActionResult> CalcularIngresoNetoFamiliarJS(int idBanco, int idEndeudamiento, decimal cuotaMensual)
        {
            try
            {
                return Json(await _calculosBusiness.CalcularIngresoNetoFamiliar(idBanco, idEndeudamiento, cuotaMensual));
            }
            catch (Exception ex)
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
            catch (Exception ex)
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
