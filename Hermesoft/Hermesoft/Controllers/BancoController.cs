using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace HermeSoft_Fusion.Controllers
{
    public class BancoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly BancoRepository _bancoRepository;
        private readonly IWebHostEnvironment _environment;

        public BancoController(
            AppDbContext context,
            BancoRepository bancoRepository,
            IWebHostEnvironment environment)
        {
            _context = context;
            _bancoRepository = bancoRepository;
            _environment = environment;
        }

        private decimal? ParseDecimalFlexible(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            raw = raw.Trim();

            // acepta "5,25" o "5.25"
            raw = raw.Replace(",", ".");

            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                return val;

            return null;
        }

        public async Task<IActionResult> Index()
        {
            var bancos = await _bancoRepository.ObtenerTodos();
            return View(bancos);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var banco = await _bancoRepository.ObtenerPorId(id);
            if (banco == null)
            {
                TempData["MensajeError"] = "El banco no existe o ha sido eliminado.";
                return RedirectToAction("Index");
            }

            ViewBag.Endeudamientos = await _context.ENDEUDAMIENTOS_MAXIMOS
                .Include(e => e.TipoAsalariado)
                .Where(e => e.IdBanco == id)
                .ToListAsync();

            ViewBag.Seguros = await _context.SEGUROS_BANCOS
                .Include(s => s.Seguro)
                .Where(s => s.IdBanco == id)
                .ToListAsync();

            ViewBag.Escenarios = await _context.ESCENARIOS_TASAS_INTERES
                .Include(e => e.TasaInteres)
                .Where(e => e.IdBanco == id)
                .ToListAsync();

            return View(banco);
        }

        [HttpGet]
        public IActionResult Registro()
        {
            TempData.Clear();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(
            Banco banco,
            IFormFile LogoFile,
            decimal? endeudamientoPublico,
            decimal? endeudamientoPrivado,
            decimal? endeudamientoProfesional,
            decimal? endeudamientoIndependiente,
            decimal? seguroDesempleo,
            decimal? seguroVida,
            string escenariosJson
        )
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(banco.Nombre) || string.IsNullOrWhiteSpace(banco.Enlace))
            {
                TempData["MensajeError"] = "Debe completar los campos requeridos.";
                return View(banco);
            }

            bool existe = await _context.BANCOS
                .AnyAsync(b => b.Nombre == banco.Nombre || b.Enlace == banco.Enlace);

            if (existe)
            {
                TempData["MensajeError"] = "Ya existe un banco con ese nombre o enlace.";
                return View(banco);
            }

            if (string.IsNullOrWhiteSpace(escenariosJson))
            {
                TempData["MensajeError"] = "Debe agregar al menos un escenario de tasas.";
                return View(banco);
            }

            List<EscenarioRegistroDto> escenarios;
            try
            {
                escenarios = JsonSerializer.Deserialize<List<EscenarioRegistroDto>>(
                    escenariosJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new();
            }
            catch
            {
                TempData["MensajeError"] = "Error al procesar los escenarios.";
                return View(banco);
            }

            if (escenarios.Count == 0)
            {
                TempData["MensajeError"] = "Debe agregar al menos un escenario.";
                return View(banco);
            }

            // Reglas de negocio
            foreach (var esc in escenarios)
            {
                if (string.IsNullOrWhiteSpace(esc.Nombre))
                {
                    TempData["MensajeError"] = "Todos los escenarios deben tener nombre.";
                    return View(banco);
                }

                if (esc.TipoTasa == "Tasa_Variable" && esc.Tramos.Count != 1)
                {
                    TempData["MensajeError"] = "La tasa variable solo permite un tramo.";
                    return View(banco);
                }

                if (esc.TipoTasa == "Tasa_Escalonada" && esc.Tramos.Count < 1)
                {
                    TempData["MensajeError"] = "La tasa escalonada debe tener tramos.";
                    return View(banco);
                }
            }

            try
            {
                // Logo
                if (LogoFile != null && LogoFile.Length > 0)
                {
                    var folder = Path.Combine(_environment.WebRootPath, "assets/images/bancos");
                    Directory.CreateDirectory(folder);

                    var fileName = Guid.NewGuid() + Path.GetExtension(LogoFile.FileName);
                    var path = Path.Combine(folder, fileName);

                    using var stream = new FileStream(path, FileMode.Create);
                    await LogoFile.CopyToAsync(stream);

                    banco.Logo = "/assets/images/bancos/" + fileName;
                }

                // Guardar banco
                _context.BANCOS.Add(banco);
                await _context.SaveChangesAsync();
                int idBanco = banco.IdBanco;

                // Endeudamientos
                if (endeudamientoPublico.HasValue)
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo { IdBanco = idBanco, IdTipoAsalariado = 1, PorcEndeudamiento = endeudamientoPublico.Value });

                if (endeudamientoPrivado.HasValue)
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo { IdBanco = idBanco, IdTipoAsalariado = 2, PorcEndeudamiento = endeudamientoPrivado.Value });

                if (endeudamientoProfesional.HasValue)
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo { IdBanco = idBanco, IdTipoAsalariado = 3, PorcEndeudamiento = endeudamientoProfesional.Value });

                if (endeudamientoIndependiente.HasValue)
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo { IdBanco = idBanco, IdTipoAsalariado = 4, PorcEndeudamiento = endeudamientoIndependiente.Value });

                // Seguros
                if (seguroDesempleo.HasValue)
                    _context.SEGUROS_BANCOS.Add(new SeguroBanco { IdBanco = idBanco, IdSeguro = 1, PorcSeguro = seguroDesempleo.Value });

                if (seguroVida.HasValue)
                    _context.SEGUROS_BANCOS.Add(new SeguroBanco { IdBanco = idBanco, IdSeguro = 2, PorcSeguro = seguroVida.Value });

                // Escenarios y tramos
                foreach (var esc in escenarios)
                {
                    int idTasa = esc.TipoTasa == "Tasa_Variable" ? 1 : 2;

                    foreach (var tramo in esc.Tramos)
                    {
                        _context.ESCENARIOS_TASAS_INTERES.Add(new EscenarioTasaInteres
                        {
                            IdBanco = idBanco,
                            IdTasaInteres = idTasa,
                            Nombre = esc.Nombre,
                            Plazo = tramo.Plazo,
                            PorcAdicional = tramo.PorcentajeAdicional,
                            PorcDatoBancario = MapIndicador(tramo.Indicador)
                        });
                    }
                }

                await _context.SaveChangesAsync();

                TempData["MensajeExito"] = "Banco registrado exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al registrar: " + ex.Message;
                return View(banco);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Editar(
            Banco banco,
            IFormFile LogoFile,
            decimal? endeudamientoPublico, decimal? endeudamientoPrivado,
            decimal? endeudamientoProfesional, decimal? endeudamientoIndependiente,
            decimal? seguroDesempleo, decimal? seguroVida,
            string escenariosJson
         )
        {
            // 1) Traer banco actual
            var bancoActual = await _context.BANCOS.FirstOrDefaultAsync(b => b.IdBanco == banco.IdBanco);
            if (bancoActual == null)
            {
                TempData["MensajeError"] = "El banco no existe o fue eliminado.";
                return RedirectToAction("Index");
            }

            // 2) Snapshot ANTES (para histórico)
            var antesEndeudamientos = await _context.ENDEUDAMIENTOS_MAXIMOS
                .Where(e => e.IdBanco == banco.IdBanco).ToListAsync();

            var antesSeguros = await _context.SEGUROS_BANCOS
                .Where(s => s.IdBanco == banco.IdBanco).ToListAsync();

            var antesEscenarios = await _context.ESCENARIOS_TASAS_INTERES
                .Where(e => e.IdBanco == banco.IdBanco).ToListAsync();

            var infoAntes = new
            {
                Banco = bancoActual,
                Endeudamientos = antesEndeudamientos,
                Seguros = antesSeguros,
                Escenarios = antesEscenarios
            };

            // 3) Validaciones básicas
            if (string.IsNullOrWhiteSpace(banco.Nombre) || string.IsNullOrWhiteSpace(banco.Enlace))
            {
                TempData["MensajeError"] = "Debe completar los campos requeridos.";
                return View(bancoActual);
            }

            // Duplicados (excluyendo el propio banco)
            bool existe = await _context.BANCOS.AnyAsync(b =>
                (b.Nombre == banco.Nombre || b.Enlace == banco.Enlace) && b.IdBanco != banco.IdBanco);

            if (existe)
            {
                TempData["MensajeError"] = "Ya existe otro banco con ese nombre o enlace.";
                return View(bancoActual);
            }

            // Escenarios obligatorios
            if (string.IsNullOrWhiteSpace(escenariosJson))
            {
                TempData["MensajeError"] = "Debe agregar al menos un escenario de tasas.";
                return View(bancoActual);
            }

            List<EscenarioRegistroDto> escenarios;
            try
            {
                escenarios = JsonSerializer.Deserialize<List<EscenarioRegistroDto>>(
                    escenariosJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? new List<EscenarioRegistroDto>();
            }
            catch
            {
                TempData["MensajeError"] = "Error al procesar escenarios. Revise la información.";
                return View(bancoActual);
            }

            if (escenarios.Count == 0)
            {
                TempData["MensajeError"] = "Debe agregar al menos un escenario.";
                return View(bancoActual);
            }

            // Reglas negocio variable/escalonada
            foreach (var esc in escenarios)
            {
                if (string.IsNullOrWhiteSpace(esc.Nombre))
                {
                    TempData["MensajeError"] = "Todos los escenarios deben tener nombre.";
                    return View(bancoActual);
                }

                if (esc.Tramos == null || esc.Tramos.Count == 0)
                {
                    TempData["MensajeError"] = "Cada escenario debe tener al menos un tramo.";
                    return View(bancoActual);
                }

                if (esc.TipoTasa == "Tasa_Variable" && esc.Tramos.Count != 1)
                {
                    TempData["MensajeError"] = "La tasa variable solo permite un tramo.";
                    return View(bancoActual);
                }

                if (esc.TipoTasa == "Tasa_Escalonada" && esc.Tramos.Count < 1)
                {
                    TempData["MensajeError"] = "La tasa escalonada debe tener tramos.";
                    return View(bancoActual);
                }

                foreach (var tramo in esc.Tramos)
                {
                    if (tramo.Plazo <= 0)
                    {
                        TempData["MensajeError"] = "Hay tramos con plazo inválido.";
                        return View(bancoActual);
                    }
                }
            }

            try
            {
                // 4) Actualizar banco (sin perder logo si no sube uno nuevo)
                bancoActual.Nombre = banco.Nombre;
                bancoActual.Enlace = banco.Enlace;
                bancoActual.MaxCredito = banco.MaxCredito;
                bancoActual.Comision = banco.Comision;
                bancoActual.TipoCambio = banco.TipoCambio;
                bancoActual.HonorarioAbogado = banco.HonorarioAbogado;


                // Logo nuevo (opcional)
                if (LogoFile != null && LogoFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "assets", "images", "bancos");
                    Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(LogoFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await LogoFile.CopyToAsync(stream);
                    }

                    bancoActual.Logo = "/assets/images/bancos/" + uniqueFileName;
                }

                // 5) BORRAR lo anterior (endeudamientos, seguros, escenarios)
                _context.ENDEUDAMIENTOS_MAXIMOS.RemoveRange(antesEndeudamientos);
                _context.SEGUROS_BANCOS.RemoveRange(antesSeguros);
                _context.ESCENARIOS_TASAS_INTERES.RemoveRange(antesEscenarios);

                // 6) INSERTAR lo nuevo (endeudamientos)
                if (endeudamientoPublico.HasValue)
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo { IdBanco = bancoActual.IdBanco, IdTipoAsalariado = 1, PorcEndeudamiento = endeudamientoPublico.Value });

                if (endeudamientoPrivado.HasValue)
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo { IdBanco = bancoActual.IdBanco, IdTipoAsalariado = 2, PorcEndeudamiento = endeudamientoPrivado.Value });

                if (endeudamientoProfesional.HasValue)
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo { IdBanco = bancoActual.IdBanco, IdTipoAsalariado = 3, PorcEndeudamiento = endeudamientoProfesional.Value });

                if (endeudamientoIndependiente.HasValue)
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo { IdBanco = bancoActual.IdBanco, IdTipoAsalariado = 4, PorcEndeudamiento = endeudamientoIndependiente.Value });

                // 7) INSERTAR lo nuevo (seguros)
                var idSeguroDesempleo = await _context.SEGUROS
                    .Where(s => s.Nombre.ToLower().Contains("desemple"))
                    .Select(s => s.IdSeguro)
                    .FirstOrDefaultAsync();

                var idSeguroVida = await _context.SEGUROS
                    .Where(s => s.Nombre.ToLower().Contains("vida"))
                    .Select(s => s.IdSeguro)
                    .FirstOrDefaultAsync();

                if (seguroDesempleo.HasValue && idSeguroDesempleo > 0)
                    _context.SEGUROS_BANCOS.Add(new SeguroBanco
                    {
                        IdBanco = bancoActual.IdBanco,
                        IdSeguro = idSeguroDesempleo,
                        PorcSeguro = seguroDesempleo.Value
                    });

                if (seguroVida.HasValue && idSeguroVida > 0)
                    _context.SEGUROS_BANCOS.Add(new SeguroBanco
                    {
                        IdBanco = bancoActual.IdBanco,
                        IdSeguro = idSeguroVida,
                        PorcSeguro = seguroVida.Value
                    });

                // 8) INSERTAR lo nuevo (escenarios y tramos)
                foreach (var esc in escenarios)
                {
                    int idTasa = esc.TipoTasa == "Tasa_Variable" ? 1 : 2;

                    foreach (var tramo in esc.Tramos)
                    {
                        _context.ESCENARIOS_TASAS_INTERES.Add(new EscenarioTasaInteres
                        {
                            IdBanco = bancoActual.IdBanco,
                            IdTasaInteres = idTasa,
                            Nombre = esc.Nombre,
                            Plazo = tramo.Plazo,
                            PorcAdicional = tramo.PorcentajeAdicional,
                            PorcDatoBancario = MapIndicadorAPorcentaje(tramo.Indicador)
                        });
                    }
                }

                await _context.SaveChangesAsync();

                // 9) Snapshot DESPUÉS (para histórico)
                var despuesEndeudamientos = await _context.ENDEUDAMIENTOS_MAXIMOS
                    .Where(e => e.IdBanco == bancoActual.IdBanco).ToListAsync();

                var despuesSeguros = await _context.SEGUROS_BANCOS
                    .Where(s => s.IdBanco == bancoActual.IdBanco).ToListAsync();

                var despuesEscenarios = await _context.ESCENARIOS_TASAS_INTERES
                    .Where(e => e.IdBanco == bancoActual.IdBanco).ToListAsync();

                var infoDespues = new
                {
                    Banco = bancoActual,
                    Endeudamientos = despuesEndeudamientos,
                    Seguros = despuesSeguros,
                    Escenarios = despuesEscenarios
                };

                // 10) Guardar histórico en BD
                string usuarioNombre = User?.Identity?.IsAuthenticated == true ? (User.Identity?.Name ?? "Administrador") : "Administrador";
                string usuarioCorreo = "admin@hermesoft.com"; // placeholder (lo conectas al login luego)

                _context.HISTORICO_CAMBIOS_BANCARIOS.Add(new HistoricoCambioBancario
                {
                    IdBanco = bancoActual.IdBanco,
                    UsuarioNombre = usuarioNombre,
                    UsuarioCorreo = usuarioCorreo,
                    TablaAfectada = "BANCOS",
                    InformacionAnterior = JsonSerializer.Serialize(infoAntes),
                    InformacionNueva = JsonSerializer.Serialize(infoDespues)
                });

                await _context.SaveChangesAsync();

                TempData["MensajeExito"] = "Banco actualizado exitosamente.";
                return RedirectToAction("Detalle", new { id = bancoActual.IdBanco });
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al editar el banco: " + ex.Message;
                return View(bancoActual);
            }
        }
        private decimal MapIndicadorAPorcentaje(string indicador)
        {
            return indicador switch
            {
                "TBP" => 8.50m,
                "SOFT" => 7.25m,
                "TPR" => 9.00m,
                "N/A" => 0m,
                _ => 0m
            };
        }


        private decimal MapIndicador(string indicador)
        {
            return indicador switch
            {
                "TBP" => 8.50m,
                "SOFT" => 7.25m,
                "TPR" => 9.00m,
                _ => 0m
            };
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var banco = await _context.BANCOS.FirstOrDefaultAsync(b => b.IdBanco == id);
            if (banco == null)
            {
                TempData["MensajeError"] = "El banco no existe o ha sido eliminado.";
                return RedirectToAction("Index");
            }

            // Endeudamientos
            var endeudamientos = await _context.ENDEUDAMIENTOS_MAXIMOS
                .Include(e => e.TipoAsalariado)
                .Where(e => e.IdBanco == id)
                .ToListAsync();

            // Seguros reales
            var segurosBanco = await _context.SEGUROS_BANCOS
                .Where(s => s.IdBanco == id)
                .ToListAsync();

            // SOLO ESTA ASIGNACIÓN
            ViewBag.Seguros = segurosBanco;


            // Escenarios
            var escenarios = await _context.ESCENARIOS_TASAS_INTERES
                .Where(e => e.IdBanco == id)
                .ToListAsync();

            // ✅ Fix Endeudamientos (4 campos SIEMPRE)
            var endeudamientosFix = new List<EndeudamientoMaximo>
    {
        new EndeudamientoMaximo { IdBanco = id, IdTipoAsalariado = 1, PorcEndeudamiento = 0 },
        new EndeudamientoMaximo { IdBanco = id, IdTipoAsalariado = 2, PorcEndeudamiento = 0 },
        new EndeudamientoMaximo { IdBanco = id, IdTipoAsalariado = 3, PorcEndeudamiento = 0 },
        new EndeudamientoMaximo { IdBanco = id, IdTipoAsalariado = 4, PorcEndeudamiento = 0 }
    };

            foreach (var e in endeudamientos)
            {
                var item = endeudamientosFix.First(x => x.IdTipoAsalariado == e.IdTipoAsalariado);
                item.PorcEndeudamiento = e.PorcEndeudamiento;
                item.TipoAsalariado = e.TipoAsalariado;
            }

            // ✅ Valores DIRECTOS para la vista (no dependen de ID fijo ni de navigation)
            // Intento 1: buscar por nombre si existe Include
            decimal seguroDesempleoPorc = segurosBanco
                .FirstOrDefault(x => x.Seguro != null &&
                                     x.Seguro.Nombre != null &&
                                     x.Seguro.Nombre.ToLower().Contains("desemple"))?.PorcSeguro ?? 0;

            decimal seguroVidaPorc = segurosBanco
                .FirstOrDefault(x => x.Seguro != null &&
                                     x.Seguro.Nombre != null &&
                                     x.Seguro.Nombre.ToLower().Contains("vida"))?.PorcSeguro ?? 0;

            // Intento 2 (fallback): si no encontró por nombre, agarra los primeros 2 registros existentes
            // (Esto garantiza que si hay datos guardados, algo sale)
            if (seguroDesempleoPorc == 0 && segurosBanco.Count > 0)
                seguroDesempleoPorc = segurosBanco[0].PorcSeguro;

            if (seguroVidaPorc == 0 && segurosBanco.Count > 1)
                seguroVidaPorc = segurosBanco[1].PorcSeguro;

            ViewBag.Endeudamientos = endeudamientosFix;
            ViewBag.Escenarios = escenarios;

            // ✅ AQUÍ lo importante:
            ViewBag.SeguroDesempleoPorc = seguroDesempleoPorc;
            ViewBag.SeguroVidaPorc = seguroVidaPorc;

            return View(banco);
        }        
}

// DTOs
public class EscenarioRegistroDto
    {
        public string TipoTasa { get; set; } = "";
        public string Nombre { get; set; } = "";
        public List<TramoRegistroDto> Tramos { get; set; } = new();
    }

    public class TramoRegistroDto
    {
        public int Plazo { get; set; }
        public decimal PorcentajeAdicional { get; set; }
        public string Indicador { get; set; } = "";
    }
}
