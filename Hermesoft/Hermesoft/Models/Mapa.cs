using System.ComponentModel.DataAnnotations;

namespace HermeSoft_Fusion.Models
{
    public class Mapa
    {
        [Key]
        public int IdMapa { get; set; }
        public string Direccion {  get; set; }
        public string condominio { get; set; }

    }

    public class PdfRequestMap
    {
        public string ImagenBase64 { get; set; }
        public string Condominio { get; set; }
        public string TipoReporte { get; set; }
    }
}
