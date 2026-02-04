using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    public class ReferenciasEconomicasController : Controller
    {
        private readonly IndicadorBancarioRepository _repo;
        private readonly IndicadorBancarioBusiness _business;

        public ReferenciasEconomicasController(IndicadorBancarioRepository repo, IndicadorBancarioBusiness business)
        {
            _repo = repo;
            _business = business;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _repo.ObtenerTodos();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Actualizar()
        {
            await _business.ActualizarIndicadores();
            return RedirectToAction(nameof(Index));
        }
    }
}
