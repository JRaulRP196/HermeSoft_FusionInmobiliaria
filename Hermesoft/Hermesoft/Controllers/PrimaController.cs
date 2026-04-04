using HermeSoft_Fusion.Business;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    public class PrimaController : Controller
    {
        private readonly PrimaBusiness _primaBusiness;
        public PrimaController(PrimaBusiness primaBusiness)
        {
            _primaBusiness = primaBusiness;
        }
        [HttpGet]
        public async Task<IActionResult> Detalle(int idPrima)
        {
            var prima = await _primaBusiness.ObtenerPorId(idPrima);

            if (prima == null)
            {
                TempData["MensajeError"] = "No se encontró la prima solicitada.";
                return RedirectToAction("Primas", "Estadistica");
            }

            return View(prima);
        }
    }
}
