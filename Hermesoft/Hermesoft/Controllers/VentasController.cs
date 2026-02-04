using HermeSoft_Fusion.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


using Dapper;
using MySql.Data.MySqlClient;

namespace HermeSoft_Fusion.Controllers
{
    public class VentasController : Controller
    {
        private HttpClient _httpClient;

        private readonly IConfiguration _config;

        public VentasController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient();
            _config = config;
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
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return lotes?.FirstOrDefault();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerLoteJson(string lote)
        {
            var response = await _httpClient.GetAsync($"http://localhost:3000/lotes/{lote}");
            var json = await response.Content.ReadAsStringAsync();

            return Content(json, "application/json");
        }
        #endregion

        
        [HttpGet]
        public async Task<IActionResult> CalcularGastoFormalizacion(
            int idBanco,
            string lote,
            decimal seguroVida,
            decimal seguroDesempleo)
        {
            // Escenario 3
            if (idBanco <= 0 || string.IsNullOrWhiteSpace(lote))
                return BadRequest("Debe seleccionar banco y lote.");

            // 1) Traer el lote desde tu API (precioVenta)
            var loteData = await ObtenerLote(lote);
            if (loteData == null)
                return NotFound("Lote no encontrado.");

            // 2) Traer comision + honorarioAbogado desde MySQL 
            var cs = _config.GetConnectionString("MySqlConnection");

            using var conn = new MySqlConnection(cs);

            var banco = await conn.QueryFirstOrDefaultAsync<dynamic>(
                @"SELECT comision, honorarioAbogado
                  FROM bancos
                  WHERE idBanco = @idBanco",
                new { idBanco });

            if (banco == null)
                return NotFound("Banco no encontrado.");

            // 3) Timbre fiscal fijo (0.5%)
            decimal timbreFiscal = 0.5m;

           
            decimal comision = (decimal)banco.comision;
            decimal honorario = (decimal)banco.honorarioAbogado;

            decimal porcentajeTotal =
                seguroVida +
                seguroDesempleo +
                honorario +
                comision +
                timbreFiscal;

            
            decimal monto = loteData.PrecioVenta * (porcentajeTotal / 100m);

            return Json(new
            {
                porcentajeTotal,
                monto,
                detalle = new
                {
                    seguroVida,
                    seguroDesempleo,
                    honorarioAbogado = honorario,
                    comision,
                    timbreFiscal
                },
                precioVenta = loteData.PrecioVenta
            });
        }
    }
}
