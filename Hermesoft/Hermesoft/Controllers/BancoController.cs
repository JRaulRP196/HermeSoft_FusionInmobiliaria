using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Models.Banco;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    public class BancoController : Controller
    {
        private readonly BancoBusiness _bancoBusiness;
        private readonly TasaInteresBusiness _tasaInteresBusiness;
        private readonly IndicadoresBancariosBusiness _indicadoresBancariosBusiness;
        private readonly TipoCambioBusiness _tipoCambioBusiness;
        public BancoController(BancoBusiness bancoBusiness, TasaInteresBusiness tasaInteresBusiness,
            IndicadoresBancariosBusiness indicadoresBancariosBusiness, TipoCambioBusiness tipoCambioBusiness)
        {
            _bancoBusiness = bancoBusiness;
            _tasaInteresBusiness = tasaInteresBusiness;
            _indicadoresBancariosBusiness = indicadoresBancariosBusiness;
            _tipoCambioBusiness = tipoCambioBusiness;
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
            ViewBag.TasaInteres = await _tasaInteresBusiness.Obtener();
            ViewBag.IndicadoresBancarios = await _indicadoresBancariosBusiness.Obtener();
            ViewBag.TipoCambio = await _tipoCambioBusiness.Obtener();
            return View(await _bancoBusiness.IniciarBanco());
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Banco banco, IFormFile LogoFile)
        {
            try
            {
                if (LogoFile == null || LogoFile.Length == 0)
                {
                    TempData["MensajeError"] = "Debe seleccionar un logo válido.";
                    return View(banco);
                }
                Banco bancoRespuesta = await _bancoBusiness.Agregar(banco, LogoFile);
                if (bancoRespuesta.IdBanco == -1)
                {
                    TempData["MensajeError"] = "Ya este banco existe";
                    return RedirectToAction("Registro");
                }
                TempData["MensajeExito"] = "Banco registrado correctamente";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["MensajeError"] = "Ocurrio un error interno a la hora de registrar un banco";
                return RedirectToAction("Registro");
            }
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
            ViewBag.IndicadoresBancarios = await _indicadoresBancariosBusiness.Obtener();
            ViewBag.TipoCambio = await _tipoCambioBusiness.Obtener();
            return View(banco);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Banco banco, IFormFile LogoFile)
        {
            try
            {
                var bancoRespuesta = await _bancoBusiness.Editar(banco, LogoFile);
                if (bancoRespuesta != null)
                {
                    TempData["MensajeExito"] = "Banco editado correctamente";
                    return RedirectToAction("Index");
                }
                TempData["MensajeError"] = "Ocurrio un error a la hora de editar el banco";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["MensajeError"] = "Ocurrio un error interno a la hora de editar el banco";
                return RedirectToAction("Index");
            }
        }

    }
}
