using HermeSoft_Fusion.Business;
using HermeSoft_Fusion.Business.Usuarios;
using HermeSoft_Fusion.Models.Usuarios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

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

            if(user.Estado == false)
            {
                TempData["MensajeError"] = "Este usuario no tiene acceso al sistema";
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

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }

        public IActionResult NuevoPassword([FromQuery] string token)
        {
            ViewBag.Token = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NuevoPassword(Usuario usuario, string token)
        {
            if (await _usuarioBusiness.CambiarPassword(usuario, token) == null)
            {
                TempData["MensajeError"] = "Enlace expirado o modificado, solicita uno nuevo";
                return View();
            }
            TempData["MensajeExito"] = "Contraseña nueva, inicia sesión con tus nuevas credenciales";
            return RedirectToAction("Login");
        }

        public IActionResult Recoverpw()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Recoverpw(Usuario usuario)
        {
            Usuario user = await _usuarioBusiness.Obtener(usuario.Correo);
            if (user == null)
            {
                TempData["MensajeError"] = "Credenciales inválidas";
                return View();
            }

            if (user.Estado == false)
            {
                TempData["MensajeError"] = "Este usuario no tiene acceso al sistema";
                return View();
            }
            var userCorreo = User.FindFirst(ClaimTypes.Email)?.Value;
            if (await _usuarioBusiness.SolicitudCambio(usuario, userCorreo) == null)
            {
                TempData["MensajeError"] = "Digita tu correo válido";
                return View();
            }
            TempData["MensajeExito"] = "Revisa tu correo";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}