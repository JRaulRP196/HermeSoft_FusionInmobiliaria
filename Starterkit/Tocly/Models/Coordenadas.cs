using System.ComponentModel.DataAnnotations;

namespace HermeSoft_Fusion.Models
{
    public class Coordenadas
    {
        [Key]
        public int IdCoordenada { get; set; }
        public string X { get; set; }
        public string  Y { get; set; }
        public string Lote { get; set; }
        public int IdMapa { get; set; }
    }
}
