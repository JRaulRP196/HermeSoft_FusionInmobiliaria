using HermeSoft_Fusion.Business;
using Microsoft.AspNetCore.Mvc;

namespace Tocly.Controllers
{
    public class AuthController : Controller
    {

        private readonly IndicadoresBancariosBusiness _indicadoresBancariosBusiness;
        private readonly TipoCambioBusiness _tipoCambioBusiness;

        public AuthController(IndicadoresBancariosBusiness indicadoresBancariosBusiness, TipoCambioBusiness tipoCambioBusiness)
        {
            _indicadoresBancariosBusiness = indicadoresBancariosBusiness;
            _tipoCambioBusiness = tipoCambioBusiness;
        }

        // GET: Auth
        public IActionResult Lockscreen()
        {
            return View();
        }
        public async Task<IActionResult> Login()
        {
            await _indicadoresBancariosBusiness.Editar();
            await _tipoCambioBusiness.Editar();
            return View();
        }
        public IActionResult Recoverpw()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
    }
}