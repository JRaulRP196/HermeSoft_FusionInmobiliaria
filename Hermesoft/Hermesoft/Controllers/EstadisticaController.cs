using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Business.Usuarios;
using HermeSoft_Fusion.Models.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize]
    public class EstadisticaController : Controller
    {

        private readonly EstadisticaBusiness _estadisticaBusiness;
        private readonly UsuarioBusiness _usuarioBusiness;

        public EstadisticaController(EstadisticaBusiness estadisticaBusiness, UsuarioBusiness usuarioBusiness)
        {
            _estadisticaBusiness = estadisticaBusiness;
            _usuarioBusiness = usuarioBusiness;
        }
        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Primas() 
        {
            Usuario usuario = await _usuarioBusiness.ObtenerConPrimas(User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value);
            return View(usuario.Ventas);
        }

        #region js

        [HttpGet]
        public async Task<IActionResult> PagosPorCondominio(string condominio, DateTime fechaInicio, DateTime fechaFinal)
        {
            try
            {
                var resultado = await _estadisticaBusiness.PagosPorCondominio(condominio, fechaInicio, fechaFinal);
                return Json(resultado);
            }
            catch (Exception e)
            {
                return Json(new
                {
                    success = false,
                    message = e.Message
                });
            }
        }

        #endregion

    }
}
