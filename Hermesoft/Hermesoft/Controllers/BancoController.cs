using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Models.ViewModels;
using HermeSoft_Fusion.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HermeSoft_Fusion.Controllers
{
    public class BancoController : Controller
    {
        private readonly AppDbContext _context;
        private BancoBusiness _bancoBusiness;
        private EndeudamientoMaximoBusiness _endeudamiento;
        private readonly IWebHostEnvironment _environment;
        private TasaInteresBusiness _tasaInteresBusiness;
        private SeguroBancoBusiness _seguroBancoBusiness;

        public BancoController(AppDbContext context, BancoBusiness bancoBusiness, IWebHostEnvironment environment,
            EndeudamientoMaximoBusiness endeudamiento, TasaInteresBusiness tasaInteresBusiness, SeguroBancoBusiness seguroBancoBusiness)
        {
            _context = context;
            _bancoBusiness = bancoBusiness;
            _environment = environment;
            _endeudamiento = endeudamiento;
            _tasaInteresBusiness = tasaInteresBusiness;
            _seguroBancoBusiness = seguroBancoBusiness;
        }

        public async Task<IActionResult> Index()
        {
            var bancos = await _bancoBusiness.ObtenerTodos();
            return View(bancos);
        }

        //public async Task<IActionResult> Detalle(int id)
        //{
        //    var banco = await _bancoRepository.ObtenerPorId(id);
        //    if (banco == null)
        //    {
        //        TempData["MensajeError"] = "El banco no existe o ha sido eliminado.";
        //        return RedirectToAction("Index");
        //    }

        //    var endeudamientoBancos = await _context.ENDEUDAMIENTOS_MAXIMOS
        //        .Include(e => e.TipoAsalariado)
        //        .Where(e => e.IdBanco == id)
        //        .ToListAsync();

        //    var segurosBancos = await _context.SEGUROS_BANCOS
        //        .Include(s => s.Seguro)
        //        .Where(s => s.IdBanco == id)
        //        .ToListAsync();

        //    var escenarios = await _context.ESCENARIOS_TASAS_INTERES
        //        .Include(e => e.TasaInteres)
        //        .Where(e => e.IdBanco == id)
        //        .ToListAsync();

        //    ViewBag.Endeudamientos = endeudamientoBancos;
        //    ViewBag.Seguros = segurosBancos;
        //    ViewBag.Escenarios = escenarios;

        //    return View(banco);
        //}

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

            return View();
        }

        public async Task<IActionResult> Editar(int id)
        {
            var banco = await _bancoBusiness.ObtenerPorId(id);
            if (banco == null)
            {
                TempData["MensajeError"] = "El banco no existe o ha sido eliminado.";
                return RedirectToAction("Index");
            }
            return View(banco);
        }
    }
}
