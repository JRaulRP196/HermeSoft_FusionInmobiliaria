using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("ESCENARIOS_TASAS_INTERES")]
    public class EscenarioTasaInteres
    {
        [Key]
        public int idEscenario { get; set; }

        public string nombre { get; set; } = "";

        public decimal porcAdicional { get; set; }
        public decimal porcDatoBancario { get; set; }

        public int plazo { get; set; }

        public int idBanco { get; set; }
        public int idTasaInteres { get; set; }
    }
}
