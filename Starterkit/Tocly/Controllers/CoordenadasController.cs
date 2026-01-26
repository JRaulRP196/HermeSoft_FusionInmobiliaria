using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Controllers
{
    public class CoordenadasController : Controller
    {

        private readonly AppDbContext _context;

        public CoordenadasController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCoordenadasPorMapa(int id)
        {
            List<Coordenadas> coordenadas = await _context.COORDENADAS.Where(c => c.IdMapa == id).ToListAsync();
            return Ok(coordenadas);
        }

    }
}
