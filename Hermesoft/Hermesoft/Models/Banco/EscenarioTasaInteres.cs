using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models.Banco
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
        public int IdBanco { get; set; }
        public int IdTasaInteres { get; set; }
        [ForeignKey("IdBanco")]
        public Banco Banco { get; set; }
        public List<PlazosEscenarios> PlazosEscenarios {  get; set; }

    }

    public class EscenarioTasaInteresRequest : EscenarioTasaInteres
    {
        public List<PlazosEscenarios> PlazosEscenarios { get; set; } = new();
    }

}
