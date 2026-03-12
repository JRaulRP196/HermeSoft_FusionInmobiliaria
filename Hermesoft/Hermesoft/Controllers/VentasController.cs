using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Business.Usuarios;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Models.Servicios;
using HermeSoft_Fusion.Models.Usuarios;
using HermeSoft_Fusion.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize(Roles = "Ventas, Administrador")]
    public class VentasController : Controller
    {
        private readonly BancoBusiness _bancoBusiness;
        private readonly LoteRepository _loteRepository;
        private readonly CalculosBusiness _calculosBusiness;
        private readonly TipoCambioBusiness _tipoCambioBusiness;
        private readonly PrimaBusiness _primaBusiness;
        private readonly VentaBusiness _ventaBusiness;
        private readonly UsuarioBusiness _usuarioBusiness;
        private readonly CondominioBusiness _condominioBusiness;

        public VentasController(BancoBusiness bancoBusiness, LoteRepository loteRepository, CalculosBusiness calculosBusiness, 
            TipoCambioBusiness tipoCambioBusiness, PrimaBusiness primaBusiness, VentaBusiness ventaBusiness, UsuarioBusiness usuarioBusiness, CondominioBusiness condominioBusiness)
        {
            _bancoBusiness = bancoBusiness;
            _loteRepository = loteRepository;
            _calculosBusiness = calculosBusiness;
            _tipoCambioBusiness = tipoCambioBusiness;
            _primaBusiness = primaBusiness;
            _ventaBusiness = ventaBusiness;
            _usuarioBusiness = usuarioBusiness;
            _condominioBusiness = condominioBusiness;
        }

        public async Task<IActionResult> Index()
        {
            Usuario usuario = await _usuarioBusiness.Obtener(User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value);
            ViewBag.Condominios = await _condominioBusiness.Obtener();
            return View(usuario.Ventas);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateTime filterDesde, DateTime filterHasta, string filterCondominio)
        {
            Usuario usuario = await _usuarioBusiness.Obtener(User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value);
            ViewBag.Condominios = await _condominioBusiness.Obtener();
            return View(await _ventaBusiness.Filtro(usuario.Ventas, filterDesde, filterHasta, filterCondominio));
        }

        public async Task<IActionResult> StepperRegistro(string lote, string cliente)
        {
            ViewBag.lote = lote;
            ViewBag.Bancos = await _bancoBusiness.ObtenerTodos();
            ViewBag.Cliente = cliente;
            return View();
        }

        public IActionResult Prima(string lote)
        {
            TempData["lote"] = lote;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AgregarPrima(Primas prima, string lote, string desgloseConDescuentoJson, 
            string desgloseSinDescuentoJson)
        {
            try
            {
                if(desgloseSinDescuentoJson != null)
                {
                    prima.DesglosesPrimas = JsonSerializer.Deserialize<List<DesglosesPrimas>>(desgloseSinDescuentoJson);
                }
                else
                {
                    prima.DesglosesPrimas = JsonSerializer.Deserialize<List<DesglosesPrimas>>(desgloseConDescuentoJson);
                }
                prima.FechaInicio = DateTime.Today;
                
                foreach(var desglose in prima.DesglosesPrimas)
                {
                    desglose.Prima = prima;
                }
                prima.Total = _calculosBusiness.CalcularTotalPrima(prima.DesglosesPrimas);
                Primas primaBD = await _primaBusiness.Agregar(prima);
                if(primaBD != null)
                {
                    TempData["MensajeExitoPrima"] = $"Prima registrada correctamente para el cliente {primaBD.CorreoCliente}, si quiere proceder con la venta selecciona esta opción";
                    TempData["Cliente"] = primaBD.CorreoCliente;
                }
                return RedirectToAction("Prima", new { lote = lote});
            }
            catch
            {
                TempData["MensajeErrorPrima"] = "Ocurrio un error interno a la hora de registrar una prima";
                return RedirectToAction("Prima", new { lote = lote});
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarVenta(Venta venta)
        {
            try
            {
                Venta ventaBD = await _ventaBusiness.Agregar(venta);
                if(ventaBD != null)
                {
                    TempData["MensajeExitoVenta"] = $"Venta registrada correctamente para el cliente {ventaBD.CorreoCliente}";
                }
                return RedirectToAction("Index", "Lote");
            }
            catch(Exception ex)
            {
                TempData["MensajeErrorVenta"] = ex.Message;
                return RedirectToAction("Index", "Lote");
            }
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
