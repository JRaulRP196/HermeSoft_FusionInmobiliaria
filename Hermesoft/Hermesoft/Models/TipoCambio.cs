using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    public class TipoCambio
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTipoCambio { get; set; }
        [Required]
        public double? Cambio { get; set; }
        public List<HermeSoft_Fusion.Models.Banco.Banco> Bancos { get; set; } = new();

    }
}
