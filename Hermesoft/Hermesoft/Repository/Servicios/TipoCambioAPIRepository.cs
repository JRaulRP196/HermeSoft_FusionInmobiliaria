using HermeSoft_Fusion.Models.Servicios;
using System.Text.Json;

namespace HermeSoft_Fusion.Repository.Servicios
{
    public class TipoCambioAPIRepository
    {

        private readonly Configuracion _cofiguracion;
        private readonly IHttpClientFactory _httpClient;

        public TipoCambioAPIRepository(Configuracion cofiguracion, IHttpClientFactory httpClient)
        {
            _cofiguracion = cofiguracion;
            _httpClient = httpClient;
        }

        public async Task<Serie> Obtener()
        {
            var endPoint = _cofiguracion.ObtenerMetodo("ApiEndPointTipoCambio", "ObtenerTipoCambio");
            var servicioTipoCambio = _httpClient.CreateClient("BCCR");
            var respuesta = await servicioTipoCambio.GetAsync(string.Format(endPoint,DateTime.Now.ToString("yyyy/MM/dd")));
            respuesta.EnsureSuccessStatusCode();
            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true};
            var resultadoDeserializado = JsonSerializer.Deserialize<TipoCambioAPI>(resultado, opciones);
            return resultadoDeserializado.Datos[0].Indicadores[0].Series[0];
        }
    }
}
