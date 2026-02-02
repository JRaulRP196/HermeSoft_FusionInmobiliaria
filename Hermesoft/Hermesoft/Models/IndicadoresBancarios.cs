using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    public class IndicadoresBancarios
    {
        [Key]
        public int IdIndicador { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PorcIndicador { get; set; }

        public DateTime? FechaVigente { get; set; }
    }
}
