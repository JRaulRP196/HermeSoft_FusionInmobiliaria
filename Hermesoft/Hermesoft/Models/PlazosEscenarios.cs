using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    public class PlazosEscenarios
    {

        [Key]
        [Required]
        public int IdPlazoEscenario { get; set; }
        [Required]
        public double PorcAdicional { get; set; }
        [Required]
        public int Plazo { get; set; }
        [Required]
        public int IdIndicador { get; set; }
        [Required]
        public int IdEscenario { get; set; }
        [ForeignKey("IdIndicador")]
        public IndicadoresBancarios Indicador { get; set; }

    }
}
