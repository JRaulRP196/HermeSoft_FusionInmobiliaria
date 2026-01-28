using HermeSoft_Fusion.Models;
using System.Text.Json;

namespace HermeSoft_Fusion.Repository
{
    public class CondominioRepository
    {

        private HttpClient _httpClient;

        public CondominioRepository(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient();
        }

        #region Utilidades

        public async Task<IEnumerable<Condominio>> Obtener()
        {
            var respuesta = await _httpClient.GetAsync($"http://localhost:3000/condominios");
            var json = await respuesta.Content.ReadAsStringAsync();

            var condominios = JsonSerializer.Deserialize<IEnumerable<Condominio>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return condominios;
        }

        #endregion

    }
}
