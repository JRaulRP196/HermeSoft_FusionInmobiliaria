using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HermeSoft_Fusion.Repository
{
    public class LoteRepository
    {

        private HttpClient _httpClient;
        private readonly AppDbContext _context;

        public LoteRepository(IHttpClientFactory httpClientFactory, AppDbContext context)
        {
            _httpClient = httpClientFactory.CreateClient();
            _context = context;
        }

        #region Utilidades

        public async Task<IEnumerable<Lote>> Obtener()
        {
            var response = await _httpClient.GetAsync($"http://localhost:3000/lotes");
            var json = await response.Content.ReadAsStringAsync();

            var lotes = JsonSerializer.Deserialize<IEnumerable<Lote>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return lotes;
        }

        public async Task<Lote> Obtener(string codigoLote)
        {
            var response = await _httpClient.GetAsync($"http://localhost:3000/lotes/{codigoLote}");
            var json = await response.Content.ReadAsStringAsync();

            var lotes = JsonSerializer.Deserialize<IEnumerable<Lote>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return lotes.FirstOrDefault();
        }

        public async Task<IEnumerable<Lote>> ObtenerPorCondominio(string condominio)
        {
            var response = await _httpClient.GetAsync($"http://localhost:3000/lotes/condominio/{condominio}");
            var json = await response.Content.ReadAsStringAsync();

            var lotes = JsonSerializer.Deserialize<IEnumerable<Lote>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return lotes;
        }

        public async Task<IEnumerable<LoteMapa>> ObtenerLotesMapa(int idMapa)
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

            return resultado.ToList();
        }

        #endregion

    }
}
