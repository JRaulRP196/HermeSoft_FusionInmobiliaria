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
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var banco = await _db.BANCOS.FirstOrDefaultAsync(b => b.idBanco == id);
            if (banco == null)
            {
                TempData["ERR"] = "Banco no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Banco = banco;

            ViewBag.TiposAsalariado = await _db.TIPOS_ASALARIADOS.OrderBy(x => x.idTipoAsalariado).ToListAsync();
            ViewBag.Seguros = await _db.SEGUROS.OrderBy(x => x.nombre).ToListAsync();
            ViewBag.Tasas = await _db.TASAS_INTERES.OrderBy(x => x.nombre).ToListAsync();

            // Datos actuales para prellenar (los usaremos en la vista)
            ViewBag.Endeudamientos = await _db.ENDEUDAMIENTOS_MAXIMOS
                .Where(x => x.idBanco == id)
                .ToListAsync();

            ViewBag.SegurosSeleccionados = await _db.SEGUROS_BANCOS
                .Where(x => x.idBanco == id)
                .Select(x => x.idSeguro)
                .ToListAsync();

            ViewBag.Escenarios = await _db.ESCENARIOS_TASAS_INTERES
                .Where(x => x.idBanco == id)
                .OrderBy(x => x.plazo)
                .ThenBy(x => x.nombre)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(
    int idBanco,
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
            // Escenario 2: obligatorios vacíos
            if (idBanco <= 0) return ErrorEditar(idBanco, "Banco inválido.");
            if (string.IsNullOrWhiteSpace(nombre)) return ErrorEditar(idBanco, "Falta el nombre del banco.");
            if (string.IsNullOrWhiteSpace(enlace)) return ErrorEditar(idBanco, "Falta el enlace web del banco.");
            if (maxCredito is null) return ErrorEditar(idBanco, "Falta el plazo máximo de crédito (años).");
            if (comision is null) return ErrorEditar(idBanco, "Falta la comisión bancaria.");
            if (honorarioAbogado is null) return ErrorEditar(idBanco, "Faltan los honorarios de abogado.");
            if (string.IsNullOrWhiteSpace(tipoCambio)) return ErrorEditar(idBanco, "Falta el tipo de cambio.");

            if (string.IsNullOrWhiteSpace(endeudamientosJson))
                return ErrorEditar(idBanco, "No se generó endeudamientosJson. (Revisa JS + hidden input).");
            if (string.IsNullOrWhiteSpace(segurosJson))
                return ErrorEditar(idBanco, "No se generó segurosJson. (Revisa JS + hidden input).");
            if (string.IsNullOrWhiteSpace(escenariosJson))
                return ErrorEditar(idBanco, "No se generó escenariosJson. (Revisa JS + hidden input).");

            // Parse JSON
            List<EndeudamientoDto>? endeudamientos;
            List<int>? seguros;
            List<EscenarioDto>? escenarios;

            try
            {
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                endeudamientos = JsonSerializer.Deserialize<List<EndeudamientoDto>>(endeudamientosJson, opts);
                seguros = JsonSerializer.Deserialize<List<int>>(segurosJson, opts);
                escenarios = JsonSerializer.Deserialize<List<EscenarioDto>>(escenariosJson, opts);
            }
            catch
            {
                return ErrorEditar(idBanco, "Error leyendo los datos del formulario (JSON).");
            }

            if (endeudamientos == null || endeudamientos.Count == 0)
                return ErrorEditar(idBanco, "Debe ingresar endeudamiento máximo por tipo de asalariado.");
            if (seguros == null || seguros.Count == 0)
                return ErrorEditar(idBanco, "Debe seleccionar al menos un seguro.");
            if (escenarios == null || escenarios.Count == 0)
                return ErrorEditar(idBanco, "Debe registrar al menos un escenario de tasa.");

            // Banco existente (excluyendo el actual)
            var nom = nombre.Trim().ToLower();
            var url = enlace.Trim().ToLower();
            bool existe = await _db.BANCOS.AnyAsync(b =>
                b.idBanco != idBanco && (b.nombre.ToLower() == nom || b.enlace.ToLower() == url));

            if (existe)
                return ErrorEditar(idBanco, "Ya existe otro banco con ese nombre o enlace.");

            // Cargar banco actual + relaciones (para histórico)
            var banco = await _db.BANCOS.FirstOrDefaultAsync(b => b.idBanco == idBanco);
            if (banco == null) return ErrorEditar(idBanco, "Banco no encontrado.");

            var oldBanco = new
            {
                banco.idBanco,
                banco.nombre,
                banco.enlace,
                banco.maxCredito,
                banco.comision,
                banco.honorarioAbogado,
                banco.tipoCambio,
                banco.logo
            };

            var oldEnd = await _db.ENDEUDAMIENTOS_MAXIMOS.Where(x => x.idBanco == idBanco).ToListAsync();
            var oldSeg = await _db.SEGUROS_BANCOS.Where(x => x.idBanco == idBanco).ToListAsync();
            var oldEsc = await _db.ESCENARIOS_TASAS_INTERES.Where(x => x.idBanco == idBanco).ToListAsync();

            // Guardar logo (si viene uno nuevo)
            string? logoPath = banco.logo;
            if (logoFile != null && logoFile.Length > 0)
            {
                var ext = Path.GetExtension(logoFile.FileName).ToLowerInvariant();
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                if (!allowed.Contains(ext))
                    return ErrorEditar(idBanco, "El logo debe ser JPG, PNG o WEBP.");

                var dir = Path.Combine(_env.WebRootPath, "logos_bancos");
                Directory.CreateDirectory(dir);

                var fileName = $"{Guid.NewGuid()}{ext}";
                var fullPath = Path.Combine(dir, fileName);

                using var stream = System.IO.File.Create(fullPath);
                await logoFile.CopyToAsync(stream);

                logoPath = $"/logos_bancos/{fileName}";
            }

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // Update Banco
                banco.nombre = nombre.Trim();
                banco.enlace = enlace.Trim();
                banco.maxCredito = maxCredito.Value;
                banco.comision = comision.Value;
                banco.honorarioAbogado = honorarioAbogado.Value;
                banco.tipoCambio = tipoCambio.Trim();
                banco.logo = logoPath;

                await _db.SaveChangesAsync();

                // Reemplazar relaciones (simple y seguro)
                _db.ENDEUDAMIENTOS_MAXIMOS.RemoveRange(oldEnd);
                _db.SEGUROS_BANCOS.RemoveRange(oldSeg);
                _db.ESCENARIOS_TASAS_INTERES.RemoveRange(oldEsc);
                await _db.SaveChangesAsync();

                foreach (var e in endeudamientos)
                {
                    _db.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo
                    {
                        idBanco = idBanco,
                        idTipoAsalariado = e.idTipoAsalariado,
                        porcEndeudamiento = e.porcEndeudamiento
                    });
                }

                foreach (var idSeguro in seguros.Distinct())
                {
                    _db.SEGUROS_BANCOS.Add(new SeguroBanco
                    {
                        idBanco = idBanco,
                        idSeguro = idSeguro
                    });
                }

                foreach (var esc in escenarios)
                {
                    _db.ESCENARIOS_TASAS_INTERES.Add(new EscenarioTasaInteres
                    {
                        idBanco = idBanco,
                        idTasaInteres = esc.idTasaInteres,
                        nombre = esc.nombre.Trim(),
                        plazo = esc.plazo,
                        porcAdicional = esc.porcAdicional,
                        porcDatoBancario = esc.porcDatoBancario
                    });
                }

                await _db.SaveChangesAsync();

                // Escenario 3: histórico (usuario)
                string usuarioNombre = User?.Identity?.Name ?? "Sistema";
                string usuarioCorreo = User?.Claims?.FirstOrDefault(c => c.Type.Contains("email"))?.Value ?? "sin-correo@local";

                var newBanco = new
                {
                    banco.idBanco,
                    banco.nombre,
                    banco.enlace,
                    banco.maxCredito,
                    banco.comision,
                    banco.honorarioAbogado,
                    banco.tipoCambio,
                    banco.logo
                };

                var newEnd = endeudamientos;
                var newSeg = seguros;
                var newEsc = escenarios;

                var infoAnterior = JsonSerializer.Serialize(new { Banco = oldBanco, Endeudamientos = oldEnd, Seguros = oldSeg, Escenarios = oldEsc });
                var infoNueva = JsonSerializer.Serialize(new { Banco = newBanco, Endeudamientos = newEnd, Seguros = newSeg, Escenarios = newEsc });

                _db.HISTORICO_CAMBIOS_BANCARIOS.Add(new HistoricoCambioBancario
                {
                    idBanco = idBanco,
                    fechaCambio = DateTime.Now,
                    usuarioNombre = usuarioNombre,
                    usuarioCorreo = usuarioCorreo,
                    tablaAfectada = "BANCOS (+ RELACIONES)",
                    informacionAnterior = infoAnterior,
                    informacionNueva = infoNueva
                });

                await _db.SaveChangesAsync();

                await tx.CommitAsync();

                TempData["OK"] = "Edición exitosa del banco.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await tx.RollbackAsync();
                return ErrorEditar(idBanco, "Ocurrió un error al editar el banco.");
            }
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

        private IActionResult ErrorEditar(int idBanco, string msg)
        {
            ViewBag.Error = msg;
            ViewBag.Banco = _db.BANCOS.FirstOrDefault(b => b.idBanco == idBanco);

            ViewBag.TiposAsalariado = _db.TIPOS_ASALARIADOS.OrderBy(x => x.idTipoAsalariado).ToList();
            ViewBag.Seguros = _db.SEGUROS.OrderBy(x => x.nombre).ToList();
            ViewBag.Tasas = _db.TASAS_INTERES.OrderBy(x => x.nombre).ToList();

            ViewBag.Endeudamientos = _db.ENDEUDAMIENTOS_MAXIMOS.Where(x => x.idBanco == idBanco).ToList();
            ViewBag.SegurosSeleccionados = _db.SEGUROS_BANCOS.Where(x => x.idBanco == idBanco).Select(x => x.idSeguro).ToList();
            ViewBag.Escenarios = _db.ESCENARIOS_TASAS_INTERES.Where(x => x.idBanco == idBanco).ToList();

            return View("Editar");
        }

    }
}
