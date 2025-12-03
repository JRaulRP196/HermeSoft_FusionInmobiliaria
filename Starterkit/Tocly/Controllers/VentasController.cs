using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    public class VentasController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Registro() => View();
        public IActionResult Detalle() => View();
        public IActionResult Editar() => View();
    }
}
