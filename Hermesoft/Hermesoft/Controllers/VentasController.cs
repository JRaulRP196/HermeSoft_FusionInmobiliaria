using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Models.Banco;
using HermeSoft_Fusion.Models.Servicios;
using HermeSoft_Fusion.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize(Roles = "Ventas, Administrador")]
    public class VentasController : Controller
    {
        private readonly BancoBusiness _bancoBusiness;
        private readonly LoteRepository _loteRepository;
        private readonly CalculosBusiness _calculosBusiness;
        private readonly TipoCambioBusiness _tipoCambioBusiness;


        public VentasController(BancoBusiness bancoBusiness, LoteRepository loteRepository, CalculosBusiness calculosBusiness, TipoCambioBusiness tipoCambioBusiness)
        {
            _bancoBusiness = bancoBusiness;
            _loteRepository = loteRepository;
            _calculosBusiness = calculosBusiness;
            _tipoCambioBusiness = tipoCambioBusiness;
        }

        public IActionResult Index() => View();
        public async Task<IActionResult> StepperRegistro(string lote)
        {
            ViewBag.lote = lote;
            ViewBag.Bancos = await _bancoBusiness.ObtenerTodos();
            return View();
        }
        public async Task<IActionResult> Registro(string lote)
        {
            ViewBag.lote = lote;
            ViewBag.Bancos = await _bancoBusiness.ObtenerTodos();
            return View();
        }

        public IActionResult Prima(string lote)
        {
            TempData["lote"] = lote;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TipoCambioActual()
        {
            var tc = await _tipoCambioBusiness.Obtener(); // esto ya existe en tu Business

            if (tc == null || tc.Cambio <= 0) // (o la propiedad equivalente)
                return Json(new { ok = false });

            return Json(new { ok = true, tipoCambio = tc.Cambio });
        }
    }
}
