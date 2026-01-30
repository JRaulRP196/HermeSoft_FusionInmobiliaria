using HermeSoft_Fusion.Business;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    public class CondominioController : Controller
    {

        private CondominioBusiness _condominioBusiness;

        public CondominioController(CondominioBusiness condominioBusiness)
        {
            _condominioBusiness = condominioBusiness;
        }

        #region Utilidades

        [HttpGet]
        public async Task<IActionResult> Obtener()
        {
            var condominios = await _condominioBusiness.Obtener();
            return Json(condominios);
        }

        #endregion

    }
}
