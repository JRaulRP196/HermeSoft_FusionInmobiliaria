using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{
    public class TablesController : Controller
    {
        // GET: Tables
        [HttpGet]
        public async Task<IActionResult> Basic()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Datatables()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Gridjs()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Listjs()
        {
            return await Task.FromResult(View());
        }
    }
}