using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Models.Banco;
using HermeSoft_Fusion.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    public class VentasController : Controller
    {
        private readonly BancoBusiness _bancoBusiness;
        private readonly LoteRepository _loteRepository;
        private readonly CalculosBusiness _calculosBusiness;

        public VentasController(BancoBusiness bancoBusiness, LoteRepository loteRepository, CalculosBusiness calculosBusiness)
        {
            _bancoBusiness = bancoBusiness;
            _loteRepository = loteRepository;
            _calculosBusiness = calculosBusiness;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> Registro(string lote)
        {
            ViewBag.lote = lote;
            ViewBag.Bancos = await _bancoBusiness.ObtenerTodos();
            return View();
        }

        
        // HU TASA - Escenario 1/2: al seleccionar escenario, traer tasa
        
        [HttpGet]
        public async Task<IActionResult> ObtenerEscenariosPorBanco(int idBanco)
        {
            var banco = await _bancoBusiness.ObtenerPorId(idBanco);
            if (banco == null) return NotFound(new { ok = false, message = "Banco no encontrado." });

          
            var escenarios = (banco.EscenariosTasaInteres ?? new List<EscenarioTasaInteres>())
                .Select(e => new
                {
                    idEscenario = e.IdEscenario,
                    nombre = e.Nombre
                })
                .ToList();

            return Json(new { ok = true, data = escenarios });
        }
        [HttpGet]
        public async Task<IActionResult> ObtenerDatosBanco(int idBanco)
        {
            if (idBanco <= 0)
                return BadRequest(new { ok = false, message = "Banco inválido." });

            var banco = await _bancoBusiness.ObtenerPorId(idBanco);
            if (banco == null)
                return NotFound(new { ok = false, message = "Banco no encontrado." });

            decimal porcVida = 0m;
            decimal porcDesempleo = 0m;

            if (banco.SeguroBancos != null)
            {
                foreach (var sb in banco.SeguroBancos)
                {
                    var nombre = (sb?.Seguro?.Nombre ?? "").ToLower();

                    if (nombre.Contains("vida"))
                        porcVida = sb.PorcSeguro;

                    if (nombre.Contains("desempleo"))
                        porcDesempleo = sb.PorcSeguro;
                }
            }

            return Json(new
            {
                ok = true,
                data = new
                {
                    comision = banco.Comision,
                    honorarioAbogado = banco.HonorarioAbogado,
                    porcVida,
                    porcDesempleo
                }
            });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerGastoFormalizacion(string codigoLote, int idBanco)
        {
            if (string.IsNullOrWhiteSpace(codigoLote) || idBanco <= 0)
                return BadRequest(new { ok = false, message = "Debe seleccionar lote y banco." });

            try
            {
                var data = await _calculosBusiness.CalcularGastoFormalizacionDesdeBD(codigoLote, idBanco);
                return Json(new { ok = true, data });
            }
            catch (Exception ex)
            {
                return BadRequest(new { ok = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPlazosPorEscenario(int idBanco, int idEscenario)
        {
            var banco = await _bancoBusiness.ObtenerPorId(idBanco);
            if (banco == null) return NotFound(new { ok = false, message = "Banco no encontrado." });

            var escenario = banco.EscenariosTasaInteres?.FirstOrDefault(e => e.IdEscenario == idEscenario);
            if (escenario == null)
                return BadRequest(new { ok = false, message = "Escenario no existe." });

            var plazos = (escenario.PlazosEscenarios ?? new List<PlazosEscenarios>())
                .Select(p => p.Plazo)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            if (!plazos.Any())
                return BadRequest(new { ok = false, message = "El escenario no tiene plazos configurados." });

            return Json(new { ok = true, data = plazos });
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerTasaInteresEscenario(int idBanco, int idEscenario, int plazoMeses)
        {
            var banco = await _bancoBusiness.ObtenerPorId(idBanco);
            if (banco == null) return NotFound(new { ok = false, message = "Banco no encontrado." });

            var escenario = banco.EscenariosTasaInteres?.FirstOrDefault(e => e.IdEscenario == idEscenario);
            if (escenario == null /*|| !escenario.Activo*/) 
                return BadRequest(new { ok = false, message = "Escenario desactivado o no existe." });

            var plazo = escenario.PlazosEscenarios?.FirstOrDefault(p => p.Plazo == plazoMeses);

            // HU TASA - Escenario 2: sin datos => error y no continuar
            if (plazo == null || plazo.Indicador == null)
                return BadRequest(new { ok = false, message = "No existen datos suficientes para calcular la tasa (plazo/indicador)." });

            //  Valor actual del indicador desde BD (ej: TBP/TPR/SOFR)
            var referencia = Convert.ToDecimal(plazo.Indicador.PorcSeguro);
            var adicional = Convert.ToDecimal(plazo.PorcAdicional);
            var tasaFinalAnual = referencia + adicional;

            return Json(new
            {
                ok = true,
                data = new
                {
                    indicador = plazo.Indicador.Nombre,
                    porcentajeReferencia = referencia,
                    porcentajeAdicional = adicional,
                    tasaFinal = tasaFinalAnual
                }
            });
        }

        
        // HU CUOTA - Sistema Francés
      
        [HttpGet]
        public async Task<IActionResult> CalcularCuotaMensualBancaria(
            string codigoLote,
            int idBanco,
            int idEscenario,
            int plazoMeses,
            decimal porcPrima)
        {
            // HU CUOTA - Escenario 2: datos incompletos
            if (string.IsNullOrWhiteSpace(codigoLote) || idBanco <= 0 || idEscenario <= 0 || plazoMeses <= 0)
                return BadRequest(new { ok = false, message = "Faltan datos obligatorios." });

            var lote = await _loteRepository.Obtener(codigoLote);
            if (lote == null)
                return BadRequest(new { ok = false, message = "Lote no encontrado." });

            var precioVenta = lote.PrecioVenta;
            if (precioVenta <= 0)
                return BadRequest(new { ok = false, message = "Precio del lote inválido." });

            var prima = Math.Round(precioVenta * (porcPrima / 100m), 2);
            var montoFinanciar = precioVenta - prima;
            if (montoFinanciar <= 0)
                return BadRequest(new { ok = false, message = "Monto a financiar inválido (precio - prima)." });

            var banco = await _bancoBusiness.ObtenerPorId(idBanco);
            if (banco == null)
                return BadRequest(new { ok = false, message = "Banco no encontrado." });

            var escenario = banco.EscenariosTasaInteres?.FirstOrDefault(e => e.IdEscenario == idEscenario);
            if (escenario == null /*|| !escenario.Activo*/)
                return BadRequest(new { ok = false, message = "Escenario desactivado o no existe." });

            var p = escenario.PlazosEscenarios?.FirstOrDefault(x => x.Plazo == plazoMeses);
            if (p?.Indicador == null)
                return BadRequest(new { ok = false, message = "No existen datos suficientes para calcular tasa/cuota." });

            var referencia = Convert.ToDecimal(p.Indicador.PorcSeguro);
            var adicional = Convert.ToDecimal(p.PorcAdicional);
            var tasaAnual = referencia + adicional;

            //  Sistema Francés: a = C0 * i / (1 - (1+i)^-n)
            // i mensual = (tasa anual / 100) / 12
            decimal CuotaFrances(decimal principal, decimal tasaAnualPorc, int nMeses)
            {
                var i = (tasaAnualPorc / 100m) / 12m;
                if (i == 0) return Math.Round(principal / nMeses, 2);

                var pow = (decimal)Math.Pow((double)(1m + i), nMeses);
                var cuota = principal * (i * pow) / (pow - 1m);
                return Math.Round(cuota, 2);
            }

            var cuota = CuotaFrances(montoFinanciar, tasaAnual, plazoMeses);

            return Json(new
            {
                ok = true,
                data = new
                {
                    plazoMeses,
                    escenario = escenario.Nombre,
                    indicador = p.Indicador.Nombre,
                    tasaFinal = tasaAnual,
                    cuotaMensual = cuota
                }
            });
        }
    }
}
