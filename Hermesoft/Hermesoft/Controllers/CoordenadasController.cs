using HermeSoft_Fusion.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize(Roles = "Ventas, Administrador")]
    public class CoordenadasController : Controller
    {

        private readonly CoordenadasBusiness _coordenadasBusiness;

        public CoordenadasController(CoordenadasBusiness coordenadasBusiness)
        {
            _coordenadasBusiness = coordenadasBusiness;
        }

        #region Vistas

        [HttpPost]
        public async Task<IActionResult> AsignarCoordenada(string lote, string X, string Y, int idMapa)
        {
            if (await _coordenadasBusiness.Agregar(lote, X, Y, idMapa) != null)
            {
                return RedirectToAction("Index", "Lote");
            }
            return RedirectToAction("Index", "Lote");
        }

        #endregion

        #region Js

        [HttpGet]
        public async Task<IActionResult> GetCoordenadasPorMapa(int id)
        {
            var coordenadas = await _coordenadasBusiness.GetCoordenadasPorMapa(id);
            return Json(coordenadas);
        }

        #endregion

    }
}
