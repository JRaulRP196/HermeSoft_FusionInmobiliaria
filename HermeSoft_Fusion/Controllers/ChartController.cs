using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{
    public class ChartController : Controller
    {
        // GET: Chart
        [HttpGet]
        public async Task<IActionResult> ApexLine()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> JsChart()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Echart()
        {
            return await Task.FromResult(View());
        }
    }
}