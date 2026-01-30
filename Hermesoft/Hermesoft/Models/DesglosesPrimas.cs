using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    public class DesglosesPrimas
    {

        [Key]
        public int IdDesglosePrima { get; set;}
        public DateTime FechaCobro {  get; set;}
        public decimal Monto { get; set;}
        public string Estado { get; set;}
        public DateTime? FechaPagado { get; set;}
        [ForeignKey("Prima")]
        public int IdPrima { get; set;}
        public Primas Prima { get; set;}
    }
}
