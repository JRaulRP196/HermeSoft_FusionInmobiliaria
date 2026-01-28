using System.ComponentModel.DataAnnotations;

namespace HermeSoft_Fusion.Models
{
    public class Primas
    {

        [Key]
        public int IdPrima { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaCierre { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal Total {  get; set; }
        public IEnumerable<DesglosesPrimas>? DesglosesPrimas { get; set; }

    }
}
