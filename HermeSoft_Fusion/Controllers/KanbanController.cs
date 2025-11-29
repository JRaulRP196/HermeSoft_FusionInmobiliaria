using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{
    public class KanbanController : Controller
    {
        // GET: Kanban
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }
    }
}