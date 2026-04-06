using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Business.Usuarios;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Models.Usuarios;
using HermeSoft_Fusion.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using HermeSoft_Fusion.Models.ViewModels;

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

        public async Task<IActionResult> StepperRegistro(string lote, string cliente) //Q
        {
            ViewBag.lote = lote;
            ViewBag.Bancos = await _bancoBusiness.ObtenerTodos();
            ViewBag.Cliente = cliente;
            ViewBag.Primas = string.IsNullOrWhiteSpace(cliente) || string.IsNullOrWhiteSpace(lote)
            ? new List<Primas>()
            : await _primaBusiness.ObtenerDisponiblesPorCorreoYLote(cliente, lote);

            return View(new Venta
            {
                CorreoCliente = cliente,
                CodLote = lote
            });
        }

        public IActionResult Prima(string lote)
        {
            var model = new PrimaViewModel
            {
                Lote = lote,
                FechaCierre = DateTime.Today
            };

            if (TempData["MensajeErrorPrima"] != null)
                model.MensajeErrorPrima = TempData["MensajeErrorPrima"]!.ToString();

            if (TempData["MensajeExitoPrima"] != null)
                model.MensajeExitoPrima = TempData["MensajeExitoPrima"]!.ToString();

            if (TempData["Cliente"] != null)
                model.Cliente = TempData["Cliente"]!.ToString();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarPrima(
    PrimaViewModel model,
    string desgloseConDescuentoJson,
    string desgloseSinDescuentoJson)
        {
            try
            {
                var prima = new Primas
                {
                    CorreoCliente = model.CorreoCliente,
                    Porcentaje = model.Porcentaje,
                    FechaCierre = model.FechaCierre,
                    FechaInicio = DateTime.Today,
                    CodLote = model.Lote,
                    Asignado = false
                };

                if (!string.IsNullOrEmpty(desgloseSinDescuentoJson))
                {
                    prima.DesglosesPrimas = JsonSerializer.Deserialize<List<DesglosesPrimas>>(desgloseSinDescuentoJson);
                }
                else
                {
                    prima.DesglosesPrimas = JsonSerializer.Deserialize<List<DesglosesPrimas>>(desgloseConDescuentoJson);
                }

                foreach (var desglose in prima.DesglosesPrimas)
                {
                    desglose.Prima = prima;
                }

                prima.Total = _calculosBusiness.CalcularTotalPrima(prima.DesglosesPrimas);

                var primaBD = await _primaBusiness.Agregar(prima);

                if (primaBD != null)
                {
                    TempData["MensajeExitoPrima"] = $"Prima registrada correctamente para el cliente {primaBD.CorreoCliente}, si quiere proceder con la venta selecciona esta opción";
                    TempData["Cliente"] = primaBD.CorreoCliente;
                }

                return RedirectToAction("Prima", new { lote = model.Lote });
            }
            catch
            {
                TempData["MensajeErrorPrima"] = "Ocurrió un error interno a la hora de registrar una prima";
                return RedirectToAction("Prima", new { lote = model.Lote });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AgregarVenta(Venta venta)
        {
            try
            {
                Console.WriteLine($"IdPrima seleccionada: {venta.IdPrima}"); //Q Borrar
                Console.WriteLine($"CorreoCliente: {venta.CorreoCliente}"); //Q Borrar
                Venta ventaBD = await _ventaBusiness.Agregar(venta);
                if (ventaBD != null)
                {
                    TempData["MensajeExitoVenta"] = $"Venta registrada correctamente para el cliente {ventaBD.CorreoCliente}";
                }
                return RedirectToAction("Index", "Lote");
            }
            catch (Exception ex)
            {
                TempData["MensajeErrorVenta"] = ex.Message;
                return RedirectToAction("Index", "Lote");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GenerarComprobante(int numContrato)
        {
            byte[] pdf = await _ventaBusiness.GenerarComprobanteVenta(numContrato);
            var rutaPdfs = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdfs");
            if (!Directory.Exists(rutaPdfs))
            {
                Directory.CreateDirectory(rutaPdfs);
            }
            var nombreArchivo = Guid.NewGuid() + ".pdf";
            var path = Path.Combine(rutaPdfs, nombreArchivo);
            System.IO.File.WriteAllBytes(path, pdf);
            var url = "/pdfs/" + nombreArchivo;
            return Json(new { url });
        }

        [HttpPost]
        public async Task<IActionResult> EnviarComprobante(string pdf, string correo)
        {
            var resultado = await _ventaBusiness.EnviarComprobante(pdf, correo);
            if (!resultado)
            {
                TempData["MensajeErrorEmail"] = "No se pudo enviar el comprobante";
                return RedirectToAction("Index");
            }
            TempData["MensajeExitoEmail"] = "Se le envio el comprobante al cliente";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AnularVenta(int numContrato, string motivo)
        {
            try
            {
                var venta = await _ventaBusiness.AnularVenta(numContrato, motivo);
                if (venta == null)
                {
                    TempData["MensajeErrorEmail"] = "No se encuentra la venta disponible";
                    return RedirectToAction("Index");
                }
                TempData["MensajeExitoEmail"] = "Venta anulada correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensajeErrorEmail"] = ex.Message;
                return RedirectToAction("Index");
            }

        }

        [HttpGet]
        public async Task<IActionResult> MotivoNulidad(int numContrato)
        {
            var motivo = await _ventaBusiness.MotivoNulidad(numContrato);
            return Json(motivo);
        }

        [HttpGet]
        public async Task<IActionResult> TipoCambioActual()
        {
            var tc = await _tipoCambioBusiness.Obtener();

            if (tc == null || tc.Cambio <= 0)
                return Json(new { ok = false });

            return Json(new { ok = true, tipoCambio = tc.Cambio });
        }
    }
}
