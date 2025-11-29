using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{
    public class MapsController : Controller
    {
        // GET: Maps
        [HttpGet]
        public async Task<IActionResult> Leaflet()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Vector()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }
    }
}