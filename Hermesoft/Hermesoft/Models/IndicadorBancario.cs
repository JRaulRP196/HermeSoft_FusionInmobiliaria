using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("indicadores_bancarios")] // si tu tabla está en minúscula como en el screenshot
    public class IndicadorBancario
    {
        [Key]
        [Column("idIndicador")]
        public int IdIndicador { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("porcSeguro")]
        public decimal PorcSeguro { get; set; }

        [Column("fechaVigente")]
        public DateTime FechaVigente { get; set; }
    }
}
