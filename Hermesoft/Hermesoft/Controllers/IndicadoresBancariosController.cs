using HermeSoft_Fusion.Business;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize(Roles = "Ventas, Administrador")]
    public class IndicadoresBancariosController : Controller
    {

        private readonly IndicadoresBancariosBusiness _indicadoresBancarios;

        public IndicadoresBancariosController(IndicadoresBancariosBusiness indicadoresBancarios)
        {
            _indicadoresBancarios = indicadoresBancarios;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _indicadoresBancarios.Obtener());
        }
    }
}
