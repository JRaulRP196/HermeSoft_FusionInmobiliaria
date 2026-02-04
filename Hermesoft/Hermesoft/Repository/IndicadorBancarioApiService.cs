using System.Net.Http.Json;

namespace HermeSoft_Fusion.Repository
{
    public class IndicadorBancarioApiService
    {
        private readonly IHttpClientFactory _factory;

        public IndicadorBancarioApiService(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        public async Task<List<IndicadorApiDto>> ObtenerIndicadores()
        {
            var client = _factory.CreateClient("LocalApi");

            // ESTE es el endpoint que acabamos de crear
            var data = await client.GetFromJsonAsync<List<IndicadorApiDto>>("api/indicadores");
            return data ?? new List<IndicadorApiDto>();
        }
    }

    public class IndicadorApiDto
    {
        public int idIndicador { get; set; }
        public string nombre { get; set; }
        public decimal porcSeguro { get; set; }
        public DateTime fechaVigente { get; set; }
    }
}
