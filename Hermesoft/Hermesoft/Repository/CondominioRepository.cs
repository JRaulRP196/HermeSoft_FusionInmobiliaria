using HermeSoft_Fusion.Models.Servicios;
using System.Text.Json;

namespace HermeSoft_Fusion.Repository
{
    public class CondominioRepository
    {

        private HttpClient _httpClient;
        private readonly string _baseUrl;

        public CondominioRepository(IHttpClientFactory httpClient, IConfiguration config)
        {
            _httpClient = httpClient.CreateClient();
            _baseUrl = config["MockApi:BaseUrl"];
        }

        #region Utilidades

        public async Task<IEnumerable<Condominio>> Obtener()
        {
            var respuesta = await _httpClient.GetAsync($"{_baseUrl}condominios");
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
