using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }
    }
}