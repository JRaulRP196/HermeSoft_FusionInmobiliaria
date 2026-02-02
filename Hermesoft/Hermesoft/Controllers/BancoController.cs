using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Hosting;

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
        public async Task<IActionResult> Registro(Banco banco, IFormFile LogoFile,
            decimal? endeudamientoPublico, decimal? endeudamientoPrivado,
            decimal? endeudamientoProfesional, decimal? endeudamientoIndependiente,
            decimal? seguroDesempleo, decimal? seguroVida,
            string tipoTasa1, string nombreEscenario1, int? plazo1, decimal? porcentajeAdicional1, string indicador1)
        {
            if (string.IsNullOrEmpty(banco.Nombre) || string.IsNullOrEmpty(banco.Enlace))
            {
                TempData["MensajeError"] = "Por favor complete los campos requeridos.";
                return View(banco);
            }

            try
            {
                // Manejar carga de imagen
                if (LogoFile != null && LogoFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "assets", "images", "bancos");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(LogoFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await LogoFile.CopyToAsync(stream);
                    }

                    banco.Logo = "/assets/images/bancos/" + uniqueFileName;
                }

                // Guardar banco
                _context.BANCOS.Add(banco);
                await _context.SaveChangesAsync();
                int idBanco = banco.IdBanco;

                // Guardar porcentajes de endeudamiento
                if (endeudamientoPublico.HasValue)
                {
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo
                    {
                        IdBanco = idBanco,
                        IdTipoAsalariado = 1,
                        PorcEndeudamiento = endeudamientoPublico.Value
                    });
                }
                if (endeudamientoPrivado.HasValue)
                {
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo
                    {
                        IdBanco = idBanco,
                        IdTipoAsalariado = 2,
                        PorcEndeudamiento = endeudamientoPrivado.Value
                    });
                }
                if (endeudamientoProfesional.HasValue)
                {
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo
                    {
                        IdBanco = idBanco,
                        IdTipoAsalariado = 3,
                        PorcEndeudamiento = endeudamientoProfesional.Value
                    });
                }
                if (endeudamientoIndependiente.HasValue)
                {
                    _context.ENDEUDAMIENTOS_MAXIMOS.Add(new EndeudamientoMaximo
                    {
                        IdBanco = idBanco,
                        IdTipoAsalariado = 4,
                        PorcEndeudamiento = endeudamientoIndependiente.Value
                    });
                }

                // Guardar seguros con sus porcentajes (cada banco define sus propios valores)
                if (seguroDesempleo.HasValue)
                {
                    _context.SEGUROS_BANCOS.Add(new SeguroBanco
                    {
                        IdBanco = idBanco,
                        IdSeguro = 1,
                        PorcSeguro = seguroDesempleo.Value
                    });
                }
                if (seguroVida.HasValue)
                {
                    _context.SEGUROS_BANCOS.Add(new SeguroBanco
                    {
                        IdBanco = idBanco,
                        IdSeguro = 2,
                        PorcSeguro = seguroVida.Value
                    });
                }

                // Guardar escenario de tasa de interes
                if (!string.IsNullOrEmpty(tipoTasa1) && !string.IsNullOrEmpty(nombreEscenario1))
                {
                    int idTasa = tipoTasa1 == "Tasa_Variable" ? 1 : 2;
                    decimal porcDatoBancario = 0;
                    
                    // convertir indicador a porcentaje
                    if (indicador1 == "TBP") porcDatoBancario = 8.50m;
                    else if (indicador1 == "SOFT") porcDatoBancario = 7.25m;
                    else if (indicador1 == "TPR") porcDatoBancario = 9.00m;
                    else if (indicador1 == "N/A") porcDatoBancario = 0;
                    
                    _context.ESCENARIOS_TASAS_INTERES.Add(new EscenarioTasaInteres
                    {
                        IdBanco = idBanco,
                        IdTasaInteres = idTasa,
                        Nombre = nombreEscenario1,
                        Plazo = plazo1 ?? 0,
                        PorcAdicional = porcentajeAdicional1 ?? 0,
                        PorcDatoBancario = porcDatoBancario
                    });
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

        public async Task<IActionResult> Editar(int id)
        {
            var banco = await _bancoRepository.ObtenerPorId(id);
            if (banco == null)
            {
                TempData["MensajeError"] = "El banco no existe o ha sido eliminado.";
                return RedirectToAction("Index");
            }
            return View(banco);
        }
    }
}
