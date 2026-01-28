using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("SEGUROS")]
    public class Seguro
    {
        [Key]
        public int idSeguro { get; set; }

        public string nombre { get; set; } = "";
        public decimal porcSeguro { get; set; }
    }
}
