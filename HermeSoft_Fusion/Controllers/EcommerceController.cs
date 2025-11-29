using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{
    public class EcommerceController : Controller
    {
        // GET: Ecommerce
        [HttpGet]
        public async Task<IActionResult> Cart()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> CreateProducts()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> CustomerDetails()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Customer()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Order()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> ProductsDetails()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> ProductsList()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Products()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Wishlist()
        {
            return await Task.FromResult(View());
        }
    }
}