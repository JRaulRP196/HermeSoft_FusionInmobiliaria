using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models.Banco
{
    public class IndicadoresBancarios
    {
        [Key]
        public int IdIndicador { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public double? PorcSeguro { get; set; }

        public DateTime? FechaVigente { get; set; }
    }
}
