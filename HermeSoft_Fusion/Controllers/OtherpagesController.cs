using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{
    public class OtherpagesController : Controller
    {
        // GET: Otherpages
        [HttpGet]
        public async Task<IActionResult> BillingSubscription()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> BlogCreate()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> BlogDetails()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> BlogList()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Faqs()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Pricing()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> PrivacyPolicy()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Starter()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> TermsConditions()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Timeline()
        {
            return await Task.FromResult(View());
        }
    }
}