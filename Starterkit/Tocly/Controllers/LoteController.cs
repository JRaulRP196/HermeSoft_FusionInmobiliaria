using HermeSoft_Fusion.Business;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    public class LoteController : Controller
    {

        private LoteBusiness _loteBusiness;

        public LoteController(LoteBusiness loteBusiness)
        {
            _loteBusiness = loteBusiness;
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
