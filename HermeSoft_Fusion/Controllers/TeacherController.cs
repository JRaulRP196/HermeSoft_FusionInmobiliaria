using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{

    public class TeacherController : Controller
    {
        // GET: Teacher
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Schedule()
        {
            return await Task.FromResult(View());
        }
    }
}