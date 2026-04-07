using HermeSoft_Fusion.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize(Roles = "Ventas, Administrador")]
    public class LoteController : Controller
    {

        private LoteBusiness _loteBusiness;
        private readonly CoordenadasBusiness _coordenadasBusiness;

        public LoteController(LoteBusiness loteBusiness, CoordenadasBusiness coordenadasBusiness)
        {
            _loteBusiness = loteBusiness;
            _coordenadasBusiness = coordenadasBusiness;
        }

        #region Vistas
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ListadoLotes(string condominio)
        {
            var lotes = await _loteBusiness.ObtenerPorCondominio(condominio);
            return View(lotes);
        }

        [HttpGet]
        public async Task<IActionResult> AsignarLote(string lote)
        {
            var lot = await _loteBusiness.Obtener(lote);
            return View(lot);
        }

        [HttpPost]
        public async Task<IActionResult> Anular(int idCoordenada)
        {
            var result = await _coordenadasBusiness.Eliminar(idCoordenada);
            if (result != null)
            {
                TempData["SuccessMapa"] = "Lote eliminado del mapa correctamente";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMapa"] = "Error al eliminar el lote del mapa";
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Js

        //Las siguientes funciones son para usar con JS
        [HttpGet]
        public async Task<IActionResult> GetLotes()
        {
            var lotes = await _loteBusiness.Obtener();
            return Json(lotes);
        }

        [HttpGet]
        public async Task<IActionResult> GetLotesMapa(int idMapa)
        {
            var lotes = await _loteBusiness.ObtenerLotesMapa(idMapa);
            return Json(lotes);
        }

        #endregion

    }
}
