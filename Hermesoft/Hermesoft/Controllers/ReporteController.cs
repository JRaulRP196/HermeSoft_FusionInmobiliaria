using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReporteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
