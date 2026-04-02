using HermeSoft_Fusion.Business;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    public class RecordatorioController : Controller
    {

        private readonly VentaBusiness _ventaBusiness;
        private readonly RecordatorioBusiness _recordatorioBusiness;
        private readonly DesglosePrimaBusiness _desglosePrimaBusiness;

        public RecordatorioController(VentaBusiness ventaBusiness, RecordatorioBusiness recordatorioBusiness, DesglosePrimaBusiness desglosePrimaBusiness)
        {
            _ventaBusiness = ventaBusiness;
            _recordatorioBusiness = recordatorioBusiness;
            _desglosePrimaBusiness = desglosePrimaBusiness;
        }

        public async Task<IActionResult> Index(int idDesglose)
        {
            var desglose = await _desglosePrimaBusiness.Obtener(idDesglose);
            return View(desglose);
        }

        public async Task<IActionResult> ConfirmacionAsesor(int IdDesglose)
        {
            var desglose = await _desglosePrimaBusiness.Obtener(IdDesglose);
            return View(desglose);
        }

        public async Task<IActionResult> ConfirmarPago(int IdDesglose)
        {
            var desglose = await _desglosePrimaBusiness.Obtener(IdDesglose);
            if (await _recordatorioBusiness.ConfirmarPago(desglose))
            {
                TempData["MensajeExitoEmail"] = "Se le envio la solicitud al asesor de ventas, favor esperar su respuesta en estos días";
                return RedirectToAction("Index", new { idDesglose = desglose.IdDesglosePrima });
            }
            TempData["MensajeErrorEmail"] = "Ocurrio un error a la hora de enviar la solicitud de confirmación de pago";
            return RedirectToAction("Index", new { idDesglose = desglose.IdDesglosePrima });
        }

        public async Task<IActionResult> ActualizarDesglose(int IdDesglose)
        {
            var desglose = await _desglosePrimaBusiness.Obtener(IdDesglose);
            await _recordatorioBusiness.ActualizarDesglose(desglose);
            return RedirectToAction("ConfirmacionAsesor", new { idDesglose = desglose.IdDesglosePrima });
        }

    }
}
