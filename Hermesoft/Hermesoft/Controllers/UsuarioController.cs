using HermeSoft_Fusion.Business.Usuarios;
using HermeSoft_Fusion.Models.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    [Authorize(Roles = "Administrador")]
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
            ViewBag.Usuarios = await _usuarioBusiness.Obtener();
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

        [HttpGet]
        public async Task<IActionResult> Editar(int idUsuario)
        {
            Usuario user = await _usuarioBusiness.Obtener(idUsuario);
            if(user == null) 
                return NotFound();
            return Json(new
            {
                user.IdUsuario,
                user.Nombre,
                user.Apellido1,
                user.Apellido2,
                user.Correo,
                user.IdRol,
                user.Estado
            });
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Usuario usuario)
        {
            if(await _usuarioBusiness.Editar(usuario) == null)
            {
                TempData["MensajeError"] = "Error al cargar el usuario";
                return RedirectToAction("Index");
            }
            TempData["MensajeExito"] = "Usuario editado correctamente";
            return RedirectToAction("Index");
        }

    }
}
