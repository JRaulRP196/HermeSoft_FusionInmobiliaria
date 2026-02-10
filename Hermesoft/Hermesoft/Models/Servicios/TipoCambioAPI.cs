namespace HermeSoft_Fusion.Models.Servicios
{
    public class TipoCambioAPI
    {
        public bool Estado { get; set; }
        public string Mensaje { get; set; }
        public List<Dato> Datos { get; set; }

    }
    public class Dato
    {
        public string Titulo { get; set; }
        public string Periodicidad { get; set; }
        public List<Indicador> Indicadores { get; set; }
    }

    public class Indicador
    {
        public string CodigoIndicador { get; set; }
        public string NombreIndicador { get; set; }
        public List<Serie> Series { get; set; }
    }

    public class Serie
    {
        public string fecha { get; set; }
        public double? valorDatoPorPeriodo { get; set; }
    }

}
