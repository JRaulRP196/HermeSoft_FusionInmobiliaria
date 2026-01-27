
using HermeSoft_Fusion.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HermeSoft_Fusion.Controllers
{
    public class VentasController : Controller
    {

        private HttpClient _httpClient;

        public VentasController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public IActionResult Index() => View();
        public async Task<IActionResult> Registro(string lote)
        {
            ViewBag.lote = lote;
            return View();
        }

        public IActionResult Detalle() => View();
        public IActionResult Editar() => View();

        #region Utilidades
        [HttpGet]
        public async Task<Lote> ObtenerLote(string lote)
        {
            var response = await _httpClient.GetAsync($"http://localhost:3000/lotes/{lote}");
            var json = await response.Content.ReadAsStringAsync();

            var lotes = JsonSerializer.Deserialize<List<Lote>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            var lot = lotes.FirstOrDefault();
            return lot;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerLoteJson(string lote)
        {
            var response = await _httpClient.GetAsync($"http://localhost:3000/lotes/{lote}");
            var json = await response.Content.ReadAsStringAsync();

            return Content(json, "application/json");
        }
        #endregion
    }
}
