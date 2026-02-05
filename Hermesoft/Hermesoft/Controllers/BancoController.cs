using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Models;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    public class BancoController : Controller
    {
        private BancoBusiness _bancoBusiness;
        private EndeudamientoMaximoBusiness _endeudamiento;
        private TasaInteresBusiness _tasaInteresBusiness;
        private SeguroBancoBusiness _seguroBancoBusiness;
        private EscenarioTasaInteresBusiness _escenarioBusiness;
        private PlazosEscenariosBusiness _plazosBusiness;

        public BancoController(BancoBusiness bancoBusiness,EndeudamientoMaximoBusiness endeudamiento,
            TasaInteresBusiness tasaInteresBusiness, SeguroBancoBusiness seguroBancoBusiness, EscenarioTasaInteresBusiness escenarioBusiness,
            PlazosEscenariosBusiness plazosBusiness)
        {
            _bancoBusiness = bancoBusiness;
            _endeudamiento = endeudamiento;
            _tasaInteresBusiness = tasaInteresBusiness;
            _seguroBancoBusiness = seguroBancoBusiness;
            _escenarioBusiness = escenarioBusiness;
            _plazosBusiness = plazosBusiness;
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(BancoRequest banco, IFormFile LogoFile, decimal endeudamientoPublico, decimal endeudamientoPrivado,
            decimal endeudamientoProfesional, decimal endeudamientoIndependiente, decimal seguroDesempleo, decimal seguroVida)
        {
            if (LogoFile == null || LogoFile.Length == 0)
            {
                TempData["MensajeError"] = "Debe seleccionar un logo válido.";
                return View(banco);
            }
            Banco bank = new Banco
            {
                Nombre = banco.Nombre,
                Enlace = banco.Enlace,
                MaxCredito = banco.MaxCredito,
                HonorarioAbogado = banco.HonorarioAbogado,
                Comision = banco.Comision,
                TipoCambio = banco.TipoCambio,
            };
            bank = await _bancoBusiness.Agregar(banco, LogoFile);

            await _endeudamiento.Agregar(endeudamientoPublico, bank.IdBanco, 1);
            await _endeudamiento.Agregar(endeudamientoPrivado, bank.IdBanco, 2);
            await _endeudamiento.Agregar(endeudamientoProfesional, bank.IdBanco, 3);
            await _endeudamiento.Agregar(endeudamientoIndependiente, bank.IdBanco, 4);

            await _seguroBancoBusiness.Agregar(seguroDesempleo, bank.IdBanco, 1);
            await _seguroBancoBusiness.Agregar(seguroVida, bank.IdBanco, 2);

            foreach (EscenarioTasaInteresRequest escenarioRequest in banco.EscenariosTasaInteres)
            {
                EscenarioTasaInteres escenario = new EscenarioTasaInteres
                {
                    Nombre = escenarioRequest.Nombre,
                    IdTasaInteres = escenarioRequest.IdTasaInteres,
                    IdBanco = bank.IdBanco,
                };
                escenario = await _escenarioBusiness.Agregar(escenario);
                
                foreach(PlazosEscenarios plazoRequest in escenarioRequest.PlazosEscenarios)
                {
                    PlazosEscenarios plazo = new PlazosEscenarios
                    {
                        PorcAdicional = plazoRequest.PorcAdicional,
                        Plazo = plazoRequest.Plazo,
                        IdIndicador = plazoRequest.IdIndicador,
                        IdEscenario = escenario.IdEscenario
                    };
                    await _plazosBusiness.Agregar(plazo);
                }
            }


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
        public async Task<IActionResult> Editar(Banco banco)
        {

            return RedirectToAction("Index");
        }

    }
}
