using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("TASAS_INTERES")]
    public class TasaInteres
    {
        [Key]
        public int idTasaInteres { get; set; }
        public string nombre { get; set; } = "";
    }
}
