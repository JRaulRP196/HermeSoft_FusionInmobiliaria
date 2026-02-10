namespace HermeSoft_Fusion.Models.Servicios
{
    public class IndicadorEconomico
    {
        public bool estado { get; set; }
        public string mensaje { get; set; }
        public List<Datos> datos { get; set; }
    }

    public class Datos
    {
        public string codigoIndicador { get; set; }
        public string nombreIndicador { get; set; }
        public List<Serie> series { get; set; }
    }

}
