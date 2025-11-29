using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Urbix.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        [HttpGet]
        public async Task<IActionResult> EmailVerify()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Signin()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Signout()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Signup()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> TwoStepVerify()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> ComingSoon()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Error()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> NotAuthorize()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> UnderMaintenance()
        {
            return await Task.FromResult(View());
        }
    }
}
