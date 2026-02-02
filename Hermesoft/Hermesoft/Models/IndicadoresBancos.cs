using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    public class IndicadoresBancos
    {
        [Key]
        public int IdIndicadorBanco { get; set; }

        public int IdBanco { get; set; }
        public int IdIndicador { get; set; }

        [ForeignKey("IdBanco")]
        public Banco Banco { get; set; }

        [ForeignKey("IdIndicador")]
        public IndicadoresBancarios Indicador { get; set; }
    }
}
