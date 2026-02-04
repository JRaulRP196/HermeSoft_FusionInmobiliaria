using HermeSoft_Fusion.Repository;
using Microsoft.AspNetCore.Mvc;

namespace HermeSoft_Fusion.Controllers
{
    [ApiController]
    [Route("api/indicadores")]
    public class IndicadoresApiController : ControllerBase
    {
        private readonly IndicadorBancarioRepository _repo;

        public IndicadoresApiController(IndicadorBancarioRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var lista = await _repo.ObtenerTodos();
            return Ok(lista);
        }
    }
}
