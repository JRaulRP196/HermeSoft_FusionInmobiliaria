using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("ESCENARIOS_TASAS_INTERES")]
    public class EscenarioTasaInteres
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdEscenario { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PorcAdicional { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PorcDatoBancario { get; set; }

        [Required]
        public int Plazo { get; set; }

        public int IdBanco { get; set; }
        [ForeignKey("IdBanco")]
        public Banco Banco { get; set; }

        public int IdTasaInteres { get; set; }
        [ForeignKey("IdTasaInteres")]
        public TasaInteres TasaInteres { get; set; }
    }
}
