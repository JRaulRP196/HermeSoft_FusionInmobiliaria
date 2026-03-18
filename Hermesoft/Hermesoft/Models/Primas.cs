using System.ComponentModel.DataAnnotations;

namespace HermeSoft_Fusion.Models
{
    public class Primas
    {

        [Key]
        public int IdPrima { get; set; }
        public string CorreoCliente { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaCierre { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal Total {  get; set; }
        public List<DesglosesPrimas>? DesglosesPrimas { get; set; }
        public Venta Venta { get; set; }

    }
}
