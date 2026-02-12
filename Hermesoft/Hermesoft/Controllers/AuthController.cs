using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Business.Usuarios;
using HermeSoft_Fusion.Models.Usuarios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Tocly.Controllers
{
    public class AuthController : Controller
    {

        private readonly IndicadoresBancariosBusiness _indicadoresBancariosBusiness;
        private readonly TipoCambioBusiness _tipoCambioBusiness;
        private readonly UsuarioBusiness _usuarioBusiness;
        private readonly PasswordService _passwordService;

        public AuthController(IndicadoresBancariosBusiness indicadoresBancariosBusiness, TipoCambioBusiness tipoCambioBusiness,
            UsuarioBusiness usuarioBusiness, PasswordService passwordService)
        {
            _indicadoresBancariosBusiness = indicadoresBancariosBusiness;
            _tipoCambioBusiness = tipoCambioBusiness;
            _usuarioBusiness = usuarioBusiness;
            _passwordService = passwordService;
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
        [HttpPost]
        public async Task<IActionResult> Login(Usuario usuario)
        {
            Usuario user = await _usuarioBusiness.Obtener(usuario.Correo);
            if (user == null)
            {
                TempData["MensajeError"] = "Credenciales inválidas";
                return View();
            }

            if (!_passwordService.VerifyPassword(user, usuario.Password, user.Password))
            {
                TempData["MensajeError"] = "Credenciales inválidas";
                return View();
            }

            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                        new Claim(ClaimTypes.Email, user.Correo),
                        new Claim(ClaimTypes.Role, user.Rol.Nombre),
                        new Claim(ClaimTypes.Name, user.Nombre)
                    };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            await HttpContext.SignInAsync(
                "Cookies",
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Dashboard");
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