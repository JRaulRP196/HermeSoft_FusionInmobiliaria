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

        private readonly AppDbContext _context;

        public MapaController(AppDbContext context)
        {
            _context = context;
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
                
                // RECORDATORIO: VALIDAR
                return RedirectToAction("Index", "Lote");
            }

            var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/mapas");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            var nombreArchivo = Guid.NewGuid() + Path.GetExtension(ImagenMapa.FileName);
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await ImagenMapa.CopyToAsync(stream);
            }

            // guardar la ruta en la BD
            mapa.Direccion = "/mapas/" + nombreArchivo;

            var mapaBD = await _context.MAPAS.FirstOrDefaultAsync(m => m.condominio == mapa.condominio);

            if ( mapaBD != null )
            {
                mapaBD.Direccion = "/mapas/" + nombreArchivo;
                _context.MAPAS.Update(mapaBD);
                if (await _context.SaveChangesAsync() <= 0)
                {
                    //VALIDAR
                    return RedirectToAction("Index", "Lote");
                }
                return RedirectToAction("Index", "Lote");
            }

            _context.MAPAS.Add(mapa);
            if (await _context.SaveChangesAsync() <= 0)
            {
                //VALIDAR
                return RedirectToAction("Index", "Lote");
            }

            return RedirectToAction("Index", "Lote");
        }

        [HttpGet]
        public async Task<IActionResult> GetMapas()
        {
            List<Mapa> mapas = await _context.MAPAS.ToListAsync();
            return Ok(mapas);
        }



    }
}
