using System.Text.Json;
using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Controllers
{
    public class BancoController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public BancoController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public IActionResult Index()
        {
            var bancos = _db.BANCOS.OrderBy(b => b.nombre).ToList();
            return View(bancos);
        }

        [HttpGet]
        public IActionResult Registro()
        {
            ViewBag.TiposAsalariado = _db.TIPOS_ASALARIADOS.OrderBy(x => x.idTipoAsalariado).ToList();
            ViewBag.Seguros = _db.SEGUROS.OrderBy(x => x.nombre).ToList();
            ViewBag.Tasas = _db.TASAS_INTERES.OrderBy(x => x.nombre).ToList();
            return View();
        }

        [HttpGet("Banco/Detalle/{id:int}")]
        public async Task<IActionResult> Detalle(int id)
        {
            var banco = await _db.BANCOS.FirstOrDefaultAsync(b => b.idBanco == id);
            if (banco == null)
            {
                TempData["ERR"] = "Banco no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var endeudamientos = await (
                from e in _db.ENDEUDAMIENTOS_MAXIMOS
                join t in _db.TIPOS_ASALARIADOS on e.idTipoAsalariado equals t.idTipoAsalariado
                where e.idBanco == id
                orderby t.nombre
                select new
                {
                    Tipo = t.nombre,
                    Porcentaje = e.porcEndeudamiento
                }
            ).ToListAsync();

            var seguros = await (
                from sb in _db.SEGUROS_BANCOS
                join s in _db.SEGUROS on sb.idSeguro equals s.idSeguro
                where sb.idBanco == id
                orderby s.nombre
                select new
                {
                    Seguro = s.nombre,
                    Porcentaje = s.porcSeguro
                }
            ).ToListAsync();

            var escenarios = await (
                from esc in _db.ESCENARIOS_TASAS_INTERES
                join t in _db.TASAS_INTERES on esc.idTasaInteres equals t.idTasaInteres
                where esc.idBanco == id
                orderby esc.plazo, esc.nombre
                select new
                {
                    Escenario = esc.nombre,
                    Tasa = t.nombre,
                    Plazo = esc.plazo,
                    Adicional = esc.porcAdicional,
                    Dato = esc.porcDatoBancario
                }
            ).ToListAsync();

            ViewBag.Endeudamientos = endeudamientos;
            ViewBag.Seguros = seguros;
            ViewBag.Escenarios = escenarios;

            return View(banco);
        }


        public class EndeudamientoDto { public int idTipoAsalariado { get; set; } public decimal porcEndeudamiento { get; set; } }
        public class EscenarioDto
        {
            public string nombre { get; set; } = "";
            public int idTasaInteres { get; set; }
            public int plazo { get; set; }
            public decimal porcAdicional { get; set; }
            public decimal porcDatoBancario { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(
            string nombre,
            string enlace,
            decimal? maxCredito,
            decimal? comision,
            decimal? honorarioAbogado,
            string tipoCambio,
            IFormFile? logoFile,
            string endeudamientosJson,
            string segurosJson,
            string escenariosJson
        )
        {
            // ✅ Escenario 2: faltantes
            if (string.IsNullOrWhiteSpace(nombre))
                return ErrorRegistro("Falta el nombre del banco.");
            if (string.IsNullOrWhiteSpace(enlace))
                return ErrorRegistro("Falta el enlace web del banco.");
            if (maxCredito is null)
                return ErrorRegistro("Falta el plazo máximo de crédito (años).");
            if (comision is null)
                return ErrorRegistro("Falta la comisión bancaria.");
            if (honorarioAbogado is null)
                return ErrorRegistro("Faltan los honorarios de abogado.");
            if (string.IsNullOrWhiteSpace(tipoCambio))
                return ErrorRegistro("Falta el tipo de cambio.");

            // Parse JSON (endeudamientos/seguros/escenarios)
            List<EndeudamientoDto>? endeudamientos;
            List<int>? seguros;
            List<EscenarioDto>? escenarios;


            if (string.IsNullOrWhiteSpace(endeudamientosJson))
                return ErrorRegistro("No se generó endeudamientosJson. (Revisa JS + hidden input).");

            if (string.IsNullOrWhiteSpace(segurosJson))
                return ErrorRegistro("No se generó segurosJson. (Revisa JS + hidden input).");

            if (string.IsNullOrWhiteSpace(escenariosJson))
                return ErrorRegistro("No se generó escenariosJson. (Revisa JS + hidden input).");

            try
            {
                endeudamientos = JsonSerializer.Deserialize<List<EndeudamientoDto>>(endeudamientosJson);
                seguros = JsonSerializer.Deserialize<List<int>>(segurosJson);
                escenarios = JsonSerializer.Deserialize<List<EscenarioDto>>(escenariosJson);
            }
            catch
            {
                return ErrorRegistro("Error leyendo los datos del formulario (JSON).");
            }

            if (endeudamientos == null || endeudamientos.Count == 0)
                return ErrorRegistro("Debe ingresar endeudamiento máximo por tipo de asalariado.");
            if (seguros == null || seguros.Count == 0)
                return ErrorRegistro("Debe seleccionar al menos un seguro.");
            if (escenarios == null || escenarios.Count == 0)
                return ErrorRegistro("Debe registrar al menos un escenario de tasa.");

            // ✅ Escenario 3: banco existente por nombre o enlace
            bool existe = await _db.BANCOS.AnyAsync(b => b.nombre == nombre.Trim() || b.enlace == enlace.Trim());
            if (existe)
                return ErrorRegistro("El banco ya existe (nombre o enlace ya registrado).");

            // Guardar logo (opcional)
            string? logoPath = null;
            if (logoFile != null && logoFile.Length > 0)
            {
                var ext = Path.GetExtension(logoFile.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                if (!allowed.Contains(ext))
                    return ErrorRegistro("El logo debe ser JPG, PNG o WEBP.");

                var dir = Path.Combine(_env.WebRootPath, "logos_bancos");
                Directory.CreateDirectory(dir);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var fullPath = Path.Combine(dir, fileName);

                using var stream = System.IO.File.Create(fullPath);
                await logoFile.CopyToAsync(stream);

                logoPath = $"/logos_bancos/{fileName}";
            }

            // ✅ Escenario 1: guardar todo en BD con transacción
            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var banco = new Banco
                {
                    nombre = nombre.Trim(),
                    enlace = enlace.Trim(),
                    maxCredito = maxCredito.Value,
                    comision = comision.Value,
                    honorarioAbogado = honorarioAbogado.Value,
                    tipoCambio = tipoCambio.Trim(),
                    logo = logoPath // si no existe la columna, comenta esta línea y la propiedad en el modelo
                };

                _db.BANCOS.Add(banco);
                await _db.SaveChangesAsync();

                foreach (var e in endeudamientos)
                {
                    _db.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo
                    {
                        idBanco = banco.idBanco,
                        idTipoAsalariado = e.idTipoAsalariado,
                        porcEndeudamiento = e.porcEndeudamiento
                    });
                }

                foreach (var idSeguro in seguros.Distinct())
                {
                    _db.SEGUROS_BANCOS.Add(new SeguroBanco
                    {
                        idBanco = banco.idBanco,
                        idSeguro = idSeguro
                    });
                }

                foreach (var esc in escenarios)
                {
                    _db.ESCENARIOS_TASAS_INTERES.Add(new EscenarioTasaInteres
                    {
                        idBanco = banco.idBanco,
                        idTasaInteres = esc.idTasaInteres,
                        nombre = esc.nombre.Trim(),
                        plazo = esc.plazo,
                        porcAdicional = esc.porcAdicional,
                        porcDatoBancario = esc.porcDatoBancario
                    });
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["OK"] = "Banco registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await tx.RollbackAsync();
                return ErrorRegistro("Ocurrió un error al registrar el banco.");
            }
        }

        private IActionResult ErrorRegistro(string msg)
        {
            ViewBag.Error = msg;
            ViewBag.TiposAsalariado = _db.TIPOS_ASALARIADOS.OrderBy(x => x.idTipoAsalariado).ToList();
            ViewBag.Seguros = _db.SEGUROS.OrderBy(x => x.nombre).ToList();
            ViewBag.Tasas = _db.TASAS_INTERES.OrderBy(x => x.nombre).ToList();
            return View("Registro");
        }
    }
}
