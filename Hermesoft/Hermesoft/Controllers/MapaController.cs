using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize(Roles = "Ventas, Administrador")]
    public class MapaController : Controller
    {

        private readonly MapaBusiness _mapaBusiness;

        public MapaController( MapaBusiness mapaBusiness)
        {
            _mapaBusiness = mapaBusiness;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Mapa mapa, IFormFile ImagenMapa)
        {
            if (ImagenMapa == null || ImagenMapa.Length <= 0 || mapa.condominio == null)
            {
                TempData["ErrorMapa"] = "El mapa tiene que ser valido";
                return RedirectToAction("Index", "Lote");
            }
            var map = await _mapaBusiness.Agregar(mapa, ImagenMapa);
            if (map == null)
            {
                TempData["ErrorMapa"] = "Ocurrio un error a la hora de guardar el mapa";
                return RedirectToAction("Index", "Lote");
            }
            TempData["SuccessMapa"] = "Mapa registrado correctamente";
            return RedirectToAction("Index", "Lote");
        }

        [HttpGet]
        public async Task<IActionResult> GetMapas()
        {
            IEnumerable<Mapa> mapas = await _mapaBusiness.GetMapas();
            return Ok(mapas);
        }



    }
}
