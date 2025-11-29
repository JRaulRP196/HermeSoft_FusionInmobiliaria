using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{
    public class SchoolController : Controller
    {
        // GET: School
        [HttpGet]
        public async Task<IActionResult> AdmissionForm()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Courses()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Exam()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Parents()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Students()
        {
            return await Task.FromResult(View());
        }
    }
}