using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }
    }
}