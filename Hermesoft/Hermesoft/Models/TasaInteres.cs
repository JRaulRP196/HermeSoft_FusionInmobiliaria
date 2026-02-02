using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("TASAS_INTERES")]
    public class TasaInteres
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTasaInteres { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
    }
}
