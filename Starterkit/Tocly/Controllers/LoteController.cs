

using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text.Json;

namespace HermeSoft_Fusion.Controllers
{
    public class LoteController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;

        public LoteController(IHttpClientFactory httpClientFactory, AppDbContext context)
        {
            _httpClient = httpClientFactory.CreateClient();
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //RECORDATORIO: TRY CATCH PARA LAS EXCEPCIONES DE CONEXION
        public async Task<IActionResult> ListadoLotes(string condominio)
        {
            var response = await _httpClient.GetAsync($"http://localhost:3000/lotes/condominio/{condominio}");
            var json = await response.Content.ReadAsStringAsync();

            var lotes = JsonSerializer.Deserialize<List<Lote>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return View(lotes);
        }

        [HttpGet]
        public async Task<IActionResult> AsignarLote(string lote)
        {
            var response = await _httpClient.GetAsync($"http://localhost:3000/lotes/{lote}");
            var json = await response.Content.ReadAsStringAsync();

            var lotes = JsonSerializer.Deserialize<List<Lote>>(json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            var lot = lotes.FirstOrDefault();
            return View(lot);
        }

        [HttpPost]
        public async Task<IActionResult> AsignarLote(string lote, string X, string Y, int idMapa)
        {

            if (X == null || Y == null || lote == null)
            {
                return RedirectToAction("AsignarLote");
            }

            Coordenadas coordenadas = new Coordenadas();
            coordenadas.IdMapa = idMapa;
            coordenadas.Lote = lote;
            coordenadas.X = X;
            coordenadas.Y = Y;
            _context.COORDENADAS.Add(coordenadas);
            if (await _context.SaveChangesAsync() > 0)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }


        //Las siguientes funciones son para usar con JS
        [HttpGet]
        public async Task<IActionResult> GetLotes()
        {
            var response = await _httpClient.GetAsync("http://localhost:3000/lotes");
            var json = await response.Content.ReadAsStringAsync();

            return Content(json, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetCondominios()
        {
            var response = await _httpClient.GetAsync("http://localhost:3000/condominios");
            var json = await response.Content.ReadAsStringAsync();

            return Content(json, "application/json"); 
        }

        [HttpGet]
        public async Task<IActionResult> GetLotesMapa(int idMapa)
        {
            var response = await _httpClient.GetAsync("http://localhost:3000/lotes");
            var json = await response.Content.ReadAsStringAsync();

            var lotesApi = JsonSerializer.Deserialize<List<Lote>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var coordenadas = await _context.COORDENADAS.Where(c => c.IdMapa == idMapa).ToListAsync();
            var resultado = from l in lotesApi
                            join c in coordenadas
                            on l.Codigo equals c.Lote
                            select new LoteMapa
                            {
                                Codigo = l.Codigo,
                                Area = l.Area,
                                Fondo = l.Fondo,
                                Frente = l.Frente,
                                PrecioM2 = l.PrecioM2,
                                PrecioLista = l.PrecioLista,
                                PrecioVenta = l.PrecioVenta,
                                Estado = l.Estado,
                                Condominio = l.Condominio,
                                X = c.X,
                                Y = c.Y
                            };

            return Json(resultado);
        }


    }
}
