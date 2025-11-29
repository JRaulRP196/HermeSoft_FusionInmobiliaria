using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{
    public class IconsController : Controller
    {
        // GET: Icons
        [HttpGet]
        public async Task<IActionResult> Bootstrap()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Remix()
        {
            return await Task.FromResult(View());
        }
    }
}