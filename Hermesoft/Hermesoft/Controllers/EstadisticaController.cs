using HermeSoft_Fusion.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EstadisticaController : Controller
    {

        private readonly EstadisticaBusiness _estadisticaBusiness;

        public EstadisticaController(EstadisticaBusiness estadisticaBusiness)
        {
            _estadisticaBusiness = estadisticaBusiness;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Primas() 
        {
            return View();
        }

        #region js

        [HttpGet]
        public async Task<IActionResult> PagosPorCondominio(string condominio)
        {
            var resultado = await _estadisticaBusiness.PagosPorCondominio(condominio);
            return Json(resultado);
        }

        #endregion

    }
}
