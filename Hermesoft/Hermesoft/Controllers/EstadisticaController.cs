using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EstadisticaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Primas() 
        {
            return View();
        }

    }
}
