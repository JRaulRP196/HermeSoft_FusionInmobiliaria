using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace HermeSoft_Fusion.Controllers
{
    public class BancoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly BancoRepository _bancoRepository;
        private readonly IWebHostEnvironment _environment;

        public BancoController(AppDbContext context, BancoRepository bancoRepository, IWebHostEnvironment environment)
        {
            _context = context;
            _bancoRepository = bancoRepository;
            _environment = environment;
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

       
        private decimal? ParseDecimalFlexible(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return null;

            raw = raw.Trim().Replace(",", ".");
            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var val))
                return val;

            return null;
        }

       
        private async Task<string?> GuardarLogoAsync(IFormFile? logoFile)
        {
            if (logoFile == null || logoFile.Length == 0) return null;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "assets", "images", "bancos");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(logoFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await logoFile.CopyToAsync(stream);
            }

            return "/assets/images/bancos/" + uniqueFileName;
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

            var endeudamientoBancos = await _context.ENDEUDAMIENTOS_MAXIMOS
                .Include(e => e.TipoAsalariado)
                .Where(e => e.IdBanco == id)
                .ToListAsync();

            var segurosBancos = await _context.SEGUROS_BANCOS
                .Include(s => s.Seguro)
                .Where(s => s.IdBanco == id)
                .ToListAsync();

            var escenarios = await _context.ESCENARIOS_TASAS_INTERES
                .Include(e => e.TasaInteres)
                .Where(e => e.IdBanco == id)
                .ToListAsync();

            ViewBag.Endeudamientos = endeudamientoBancos;
            ViewBag.Seguros = segurosBancos;
            ViewBag.Escenarios = escenarios;

            return View(banco);
        }

       

        [HttpGet]
        public IActionResult Registro()
        {
            TempData.Remove("MensajeExito");
            TempData.Remove("MensajeError");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(
            Banco banco,
            IFormFile? LogoFile,
            decimal? endeudamientoPublico, decimal? endeudamientoPrivado,
            decimal? endeudamientoProfesional, decimal? endeudamientoIndependiente,
            decimal? seguroDesempleo, decimal? seguroVida,
            string escenariosJson
        )
        {
            if (string.IsNullOrEmpty(banco.Nombre) || string.IsNullOrEmpty(banco.Enlace))
            {
                TempData["MensajeError"] = "Por favor complete los campos requeridos.";
                return View(banco);
            }

            bool existe = await _context.BANCOS.AnyAsync(b => b.Nombre == banco.Nombre || b.Enlace == banco.Enlace);
            if (existe)
            {
                TempData["MensajeError"] = "Ya existe un banco con el mismo nombre o enlace.";
                return View(banco);
            }

            if (string.IsNullOrWhiteSpace(escenariosJson))
            {
                TempData["MensajeError"] = "Debe agregar al menos un escenario de tasa de interés.";
                return View(banco);
            }

            List<EscenarioRegistroDto>? escenarios;
            try
            {
                escenarios = JsonSerializer.Deserialize<List<EscenarioRegistroDto>>(escenariosJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                TempData["MensajeError"] = "Los escenarios tienen un formato inválido. Revise la información.";
                return View(banco);
            }

            if (escenarios == null || escenarios.Count == 0)
            {
                TempData["MensajeError"] = "Debe agregar al menos un escenario de tasa de interés.";
                return View(banco);
            }

            var errores = ValidarEscenarios(escenarios);
            if (errores.Count > 0)
            {
                TempData["MensajeError"] = string.Join(" | ", errores);
                return View(banco);
            }

            try
            {
               
                var rutaLogo = await GuardarLogoAsync(LogoFile);
                if (!string.IsNullOrWhiteSpace(rutaLogo))
                    banco.Logo = rutaLogo;

                _context.BANCOS.Add(banco);
                await _context.SaveChangesAsync();

                int idBanco = banco.IdBanco;

               
                if (endeudamientoPublico.HasValue)
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo { IdBanco = idBanco, IdTipoAsalariado = 1, PorcEndeudamiento = endeudamientoPublico.Value });

                if (endeudamientoPrivado.HasValue)
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo { IdBanco = idBanco, IdTipoAsalariado = 2, PorcEndeudamiento = endeudamientoPrivado.Value });

                if (endeudamientoProfesional.HasValue)
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo { IdBanco = idBanco, IdTipoAsalariado = 3, PorcEndeudamiento = endeudamientoProfesional.Value });

                if (endeudamientoIndependiente.HasValue)
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo { IdBanco = idBanco, IdTipoAsalariado = 4, PorcEndeudamiento = endeudamientoIndependiente.Value });

               
                if (seguroDesempleo.HasValue)
                    _context.SEGUROS_BANCOS.Add(new SeguroBanco { IdBanco = idBanco, IdSeguro = 1, PorcSeguro = seguroDesempleo.Value });

                if (seguroVida.HasValue)
                    _context.SEGUROS_BANCOS.Add(new SeguroBanco { IdBanco = idBanco, IdSeguro = 2, PorcSeguro = seguroVida.Value });

                
                foreach (var esc in escenarios)
                {
                    int idTasa = (esc.TipoTasa == "Tasa_Variable") ? 1 : 2;

                    foreach (var tramo in esc.Tramos)
                    {
                        _context.ESCENARIOS_TASAS_INTERES.Add(new EscenarioTasaInteres
                        {
                            IdBanco = idBanco,
                            IdTasaInteres = idTasa,
                            Nombre = esc.Nombre,
                            Plazo = tramo.Plazo,
                            PorcAdicional = tramo.PorcentajeAdicional,
                            PorcDatoBancario = MapIndicadorAPorcentaje(tramo.Indicador)
                        });
                    }
                }

                await _context.SaveChangesAsync();

                TempData["MensajeExito"] = "Banco registrado exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al registrar el banco: " + ex.Message;
                return View(banco);
            }
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

            var endeudamientos = await _context.ENDEUDAMIENTOS_MAXIMOS
                .Include(e => e.TipoAsalariado)
                .Where(e => e.IdBanco == id)
                .ToListAsync();

            var segurosBanco = await _context.SEGUROS_BANCOS
                .Include(s => s.Seguro)
                .Where(s => s.IdBanco == id)
                .ToListAsync();

            var escenarios = await _context.ESCENARIOS_TASAS_INTERES
                .Where(e => e.IdBanco == id)
                .ToListAsync();

            
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

            ViewBag.Endeudamientos = endeudamientosFix;

           
            ViewBag.Seguros = segurosBanco;

            ViewBag.Escenarios = escenarios;

            return View(banco);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(
            Banco banco,
            IFormFile? LogoFile,
            string escenariosJson
        )
        {
            if (banco.IdBanco <= 0)
            {
                TempData["MensajeError"] = "Banco inválido.";
                return RedirectToAction("Index");
            }

            var bancoActual = await _context.BANCOS.FirstOrDefaultAsync(b => b.IdBanco == banco.IdBanco);
            if (bancoActual == null)
            {
                TempData["MensajeError"] = "El banco no existe o ha sido eliminado.";
                return RedirectToAction("Index");
            }
            
            var antes = new
            {
                Banco = bancoActual,
                Endeudamientos = await _context.ENDEUDAMIENTOS_MAXIMOS.Where(x => x.IdBanco == bancoActual.IdBanco).ToListAsync(),
                Seguros = await _context.SEGUROS_BANCOS.Where(x => x.IdBanco == bancoActual.IdBanco).ToListAsync(),
                Escenarios = await _context.ESCENARIOS_TASAS_INTERES.Where(x => x.IdBanco == bancoActual.IdBanco).ToListAsync()
            };

           
            var despues = new
            {
                Banco = bancoActual,
                Endeudamientos = await _context.ENDEUDAMIENTOS_MAXIMOS.Where(x => x.IdBanco == bancoActual.IdBanco).ToListAsync(),
                Seguros = await _context.SEGUROS_BANCOS.Where(x => x.IdBanco == bancoActual.IdBanco).ToListAsync(),
                Escenarios = await _context.ESCENARIOS_TASAS_INTERES.Where(x => x.IdBanco == bancoActual.IdBanco).ToListAsync()
            };

            
            string usuarioNombre = "Sistema";
            string usuarioCorreo = "sistema@local";

            _context.HISTORICO_CAMBIOS_BANCARIOS.Add(new HistoricoCambioBancario
            {
                IdBanco = bancoActual.IdBanco,
                FechaCambio = DateTime.Now,
                UsuarioNombre = usuarioNombre,
                UsuarioCorreo = usuarioCorreo,
                TablaAfectada = "BANCOS/RELACIONES",
                InformacionAnterior = JsonSerializer.Serialize(antes),
                InformacionNueva = JsonSerializer.Serialize(despues)
            });

            
            var seguroDesempleoVal = ParseDecimalFlexible(Request.Form["seguroDesempleo"]);
            var seguroVidaVal = ParseDecimalFlexible(Request.Form["seguroVida"]);
            var honorarioVal = ParseDecimalFlexible(Request.Form["HonorarioAbogado"]);

            var endPublicoVal = ParseDecimalFlexible(Request.Form["endeudamientoPublico"]);
            var endPrivadoVal = ParseDecimalFlexible(Request.Form["endeudamientoPrivado"]);
            var endProfesionalVal = ParseDecimalFlexible(Request.Form["endeudamientoProfesional"]);
            var endIndependienteVal = ParseDecimalFlexible(Request.Form["endeudamientoIndependiente"]);

           
            if (string.IsNullOrWhiteSpace(banco.Nombre) || string.IsNullOrWhiteSpace(banco.Enlace))
            {
                TempData["MensajeError"] = "Por favor complete los campos requeridos.";
                return RedirectToAction("Editar", new { id = banco.IdBanco });
            }

            
            if (string.IsNullOrWhiteSpace(escenariosJson))
            {
                TempData["MensajeError"] = "Debe agregar al menos un escenario de tasa de interés.";
                return RedirectToAction("Editar", new { id = banco.IdBanco });
            }

            List<EscenarioRegistroDto>? escenarios;
            try
            {
                escenarios = JsonSerializer.Deserialize<List<EscenarioRegistroDto>>(escenariosJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                TempData["MensajeError"] = "Los escenarios tienen un formato inválido.";
                return RedirectToAction("Editar", new { id = banco.IdBanco });
            }

            if (escenarios == null || escenarios.Count == 0)
            {
                TempData["MensajeError"] = "Debe agregar al menos un escenario de tasa de interés.";
                return RedirectToAction("Editar", new { id = banco.IdBanco });
            }

            var errores = ValidarEscenarios(escenarios);
            if (errores.Count > 0)
            {
                TempData["MensajeError"] = string.Join(" | ", errores);
                return RedirectToAction("Editar", new { id = banco.IdBanco });
            }

            try
            {
               
                bancoActual.Nombre = banco.Nombre;
                bancoActual.Enlace = banco.Enlace;
                bancoActual.MaxCredito = banco.MaxCredito;
                bancoActual.Comision = banco.Comision;
                bancoActual.TipoCambio = banco.TipoCambio;

                if (honorarioVal.HasValue)
                    bancoActual.HonorarioAbogado = honorarioVal.Value;

                var rutaLogo = await GuardarLogoAsync(LogoFile);
                if (!string.IsNullOrWhiteSpace(rutaLogo))
                    bancoActual.Logo = rutaLogo;

                
                var endsActuales = await _context.ENDEUDAMIENTOS_MAXIMOS
                    .Where(x => x.IdBanco == bancoActual.IdBanco)
                    .ToListAsync();

                UpsertEndeudamiento(endsActuales, bancoActual.IdBanco, 1, endPublicoVal);
                UpsertEndeudamiento(endsActuales, bancoActual.IdBanco, 2, endPrivadoVal);
                UpsertEndeudamiento(endsActuales, bancoActual.IdBanco, 3, endProfesionalVal);
                UpsertEndeudamiento(endsActuales, bancoActual.IdBanco, 4, endIndependienteVal);

                
                var segurosActuales = await _context.SEGUROS_BANCOS
                    .Where(x => x.IdBanco == bancoActual.IdBanco)
                    .ToListAsync();

                UpsertSeguro(segurosActuales, bancoActual.IdBanco, 1, seguroDesempleoVal); // desempleo
                UpsertSeguro(segurosActuales, bancoActual.IdBanco, 2, seguroVidaVal);      // vida

                
                var escViejos = await _context.ESCENARIOS_TASAS_INTERES
                    .Where(x => x.IdBanco == bancoActual.IdBanco)
                    .ToListAsync();

                if (escViejos.Count > 0)
                    _context.ESCENARIOS_TASAS_INTERES.RemoveRange(escViejos);

                foreach (var esc in escenarios)
                {
                    int idTasa = (esc.TipoTasa == "Tasa_Variable") ? 1 : 2;

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

                TempData["MensajeExito"] = "Banco actualizado exitosamente.";
                return RedirectToAction("Editar", new { id = bancoActual.IdBanco });
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al actualizar el banco: " + ex.Message;
                return RedirectToAction("Editar", new { id = banco.IdBanco });
            }
        }

        

        private void UpsertEndeudamiento(List<EndeudamientoMaximo> actuales, int idBanco, int idTipoAsalariado, decimal? val)
        {
            if (!val.HasValue) return;

            var row = actuales.FirstOrDefault(x => x.IdTipoAsalariado == idTipoAsalariado);
            if (row == null)
            {
                _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo
                {
                    IdBanco = idBanco,
                    IdTipoAsalariado = idTipoAsalariado,
                    PorcEndeudamiento = val.Value
                });
            }
            else
            {
                row.PorcEndeudamiento = val.Value;
            }
        }

        private void UpsertSeguro(List<SeguroBanco> actuales, int idBanco, int idSeguro, decimal? val)
        {
            if (!val.HasValue) return;

            var row = actuales.FirstOrDefault(x => x.IdSeguro == idSeguro);
            if (row == null)
            {
                _context.SEGUROS_BANCOS.Add(new SeguroBanco
                {
                    IdBanco = idBanco,
                    IdSeguro = idSeguro,
                    PorcSeguro = val.Value
                });
            }
            else
            {
                row.PorcSeguro = val.Value;
            }
        }

        
        private List<string> ValidarEscenarios(List<EscenarioRegistroDto> escenarios)
        {
            var errores = new List<string>();

            for (int i = 0; i < escenarios.Count; i++)
            {
                var esc = escenarios[i];

                if (string.IsNullOrWhiteSpace(esc.Nombre))
                    errores.Add($"Escenario #{i + 1}: falta el nombre.");

                if (string.IsNullOrWhiteSpace(esc.TipoTasa))
                    errores.Add($"Escenario #{i + 1}: falta el tipo de tasa.");

                if (esc.Tramos == null || esc.Tramos.Count == 0)
                    errores.Add($"Escenario #{i + 1}: debe contener al menos un tramo.");

                if (esc.TipoTasa == "Tasa_Variable" && esc.Tramos != null && esc.Tramos.Count != 1)
                    errores.Add($"Escenario #{i + 1}: la Tasa Variable solo permite 1 tramo.");

                if (esc.Tramos != null)
                {
                    for (int t = 0; t < esc.Tramos.Count; t++)
                    {
                        var tramo = esc.Tramos[t];

                        if (tramo.Plazo <= 0)
                            errores.Add($"Escenario #{i + 1}, tramo #{t + 1}: plazo inválido.");

                        if (string.IsNullOrWhiteSpace(tramo.Indicador))
                            errores.Add($"Escenario #{i + 1}, tramo #{t + 1}: indicador requerido.");
                    }
                }
            }

            return errores;
        }
    } 
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
