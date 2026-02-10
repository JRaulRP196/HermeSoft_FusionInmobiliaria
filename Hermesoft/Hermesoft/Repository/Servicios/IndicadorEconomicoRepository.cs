using HermeSoft_Fusion.Models.Servicios;
using System.Text.Json;

namespace HermeSoft_Fusion.Repository.Servicios
{
    public class IndicadorEconomicoRepository
    {

        private readonly Configuracion _configuracion;
        private readonly IHttpClientFactory _httpClient;

        public IndicadorEconomicoRepository(Configuracion configuracion, IHttpClientFactory httpClient)
        {
            _configuracion = configuracion;
            _httpClient = httpClient;
        }

        public async Task<Serie> Obtener(string nombreMetodo)
        {
            var endPoint = _configuracion.ObtenerMetodo("ApiEndPointIndicadoresBancarios", nombreMetodo);
            var indicadorServicio = _httpClient.CreateClient("BCCR");
            DateTime fechaHoy = DateTime.Now;
            var respuesta = await indicadorServicio.GetAsync(string.Format(endPoint, fechaHoy.AddDays(-5).ToString("yyyy/MM/dd"),
                fechaHoy.ToString("yyyy/MM/dd")));
            respuesta.EnsureSuccessStatusCode();
            var resultado = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var resultadoDeserializado = JsonSerializer.Deserialize<IndicadorEconomico>(resultado, opciones);
            return ObtenerSerieReciente(resultadoDeserializado);
        }

        private Serie ObtenerSerieReciente(IndicadorEconomico indicador)
        {
            List<Serie> series = indicador.datos[0].series;
            for(int i = series.Count()-1; i >= 0; i--)
            {
                if(series[i].valorDatoPorPeriodo != null)
                {
                    return series[i];
                }
            }
            return null;
        }

    }
}
