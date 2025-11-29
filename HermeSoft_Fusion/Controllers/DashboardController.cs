using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        [HttpGet]
        public async Task<IActionResult> Analytics()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Media()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> School()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Error()
        {
            return await Task.FromResult(View()); // Maps to Views/Dashboard/Error.cshtml
        }
    }
}