using HermeSoft_Fusion.Business.Usuarios;
using HermeSoft_Fusion.Models.Usuarios;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HermeSoft_Fusion.Controllers
{
    public class UsuariosController : Controller
    {

        private readonly UsuarioBusiness _usuarioBusiness;
        private readonly RolBusiness _rolBusiness;

        public UsuariosController(UsuarioBusiness usuarioBusiness, RolBusiness rolBusiness)
        {
            _usuarioBusiness = usuarioBusiness;
            _rolBusiness = rolBusiness;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Roles = await _rolBusiness.Obtener();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(Usuario usuario)
        {
            Usuario user = await _usuarioBusiness.Agregar(usuario);
            if(user.IdUsuario == -1)
            {
                TempData["MensajeError"] = "No puede haber más de un usuario con el mismo correo";
                return RedirectToAction("Index");
            }
            TempData["MensajeExito"] = "Usuario registrado correctamente, le llegara un correo pronto";
            return RedirectToAction("Index");
        }

    }
}
