using Microsoft.AspNetCore.Mvc;

namespace Urbix.Controllers
{
    public class FormController : Controller
    {
        // GET: Form
        [HttpGet]
        public async Task<IActionResult> Advanced()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> CheckboxsRadios()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Editor()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Elements()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> FileUploads()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> InputGroup()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> InputMasks()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> InputSpin()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Layout()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Range()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Select()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Validation()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        public async Task<IActionResult> Wizards()
        {
            return await Task.FromResult(View());
        }
    }
}