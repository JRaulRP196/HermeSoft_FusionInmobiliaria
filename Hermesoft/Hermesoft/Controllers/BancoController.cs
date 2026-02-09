using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Models;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    public class BancoController : Controller
    {
        private BancoBusiness _bancoBusiness;
        private TasaInteresBusiness _tasaInteresBusiness;

        public BancoController(BancoBusiness bancoBusiness, TasaInteresBusiness tasaInteresBusiness)
        {
            _bancoBusiness = bancoBusiness;
            _tasaInteresBusiness = tasaInteresBusiness;
        }

        public async Task<IActionResult> Index()
        {
            var bancos = await _bancoBusiness.ObtenerTodos();
            return View(bancos);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            Banco banco = await _bancoBusiness.ObtenerPorId(id);
            return View(banco);
        }

        [HttpGet]
        public async  Task<IActionResult> Registro()
        {
            TempData.Remove("MensajeExito");
            TempData.Remove("MensajeError");

            ViewBag.TasaInteres = await _tasaInteresBusiness.Obtener();
            return View(await _bancoBusiness.IniciarBanco());
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Banco banco, IFormFile LogoFile)
        {
            if (LogoFile == null || LogoFile.Length == 0)
            {
                TempData["MensajeError"] = "Debe seleccionar un logo válido.";
                return View(banco);
            }
            await _bancoBusiness.Agregar(banco, LogoFile);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var banco = await _bancoBusiness.ObtenerPorId(id);
            if (banco == null)
            {
                TempData["MensajeError"] = "El banco no existe o ha sido eliminado.";
                return RedirectToAction("Index");
            }
            ViewBag.TasaInteres = await _tasaInteresBusiness.Obtener();
            return View(banco);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Banco banco, IFormFile LogoFile)
        {
            await _bancoBusiness.Editar(banco, LogoFile);
            return RedirectToAction("Index");
        }

    }
}
