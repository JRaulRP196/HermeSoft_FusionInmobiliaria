using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{
    public class CalendarController : Controller
    {
        // GET: Calendar
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Placeholder for future async logic (e.g., fetching events)
            return await Task.FromResult(View());
        }
    }
}