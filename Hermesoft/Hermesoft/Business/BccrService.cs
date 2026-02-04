using System.Xml.Linq;

namespace HermeSoft_Fusion.Business
{
    public class BccrService
    {
        private readonly HttpClient _http;

        public BccrService(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
            _http.BaseAddress = new Uri(
                "https://gee.bccr.fi.cr/Indicadores/Suscripciones/WS/");
            _http.Timeout = TimeSpan.FromSeconds(20);
        }

        public async Task<decimal?> ObtenerValorIndicador(
            string url,
            string usuario,
            string token,
            string idIndicador)
        {
            var hoy = DateTime.Today.ToString("dd/MM/yyyy");

            var soap = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""
               xmlns:xsd=""http://www.w3.org/2001/XMLSchema""
               xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
  <soap:Body>
    <ObtenerIndicadoresEconomicos xmlns=""http://ws.sdde.bccr.fi.cr"">
      <Indicador>{idIndicador}</Indicador>
      <FechaInicio>{hoy}</FechaInicio>
      <FechaFinal>{hoy}</FechaFinal>
      <Nombre>{usuario}</Nombre>
      <SubNiveles>N</SubNiveles>
      <CorreoElectronico>{usuario}</CorreoElectronico>
      <Token>{token}</Token>
    </ObtenerIndicadoresEconomicos>
  </soap:Body>
</soap:Envelope>";

            var content = new StringContent(
                soap,
                System.Text.Encoding.UTF8,
                "text/xml");

            content.Headers.Add(
                "SOAPAction",
                "http://ws.sdde.bccr.fi.cr/ObtenerIndicadoresEconomicos");

            var resp = await _http.PostAsync(
                "wsindicadoreseconomicos.asmx",
                content);

            if (!resp.IsSuccessStatusCode)
                return null;

            var xml = await resp.Content.ReadAsStringAsync();
            var doc = XDocument.Parse(xml);

            var valorNode = doc.Descendants()
                .FirstOrDefault(x =>
                    x.Name.LocalName.Equals(
                        "NUM_VALOR",
                        StringComparison.OrdinalIgnoreCase));

            if (valorNode == null) return null;

            var raw = valorNode.Value?.Trim();
            if (string.IsNullOrWhiteSpace(raw)) return null;

            raw = raw.Replace(",", ".");
            if (decimal.TryParse(
                raw,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var v))
            {
                return v;
            }

            return null;
        }
    }
}
