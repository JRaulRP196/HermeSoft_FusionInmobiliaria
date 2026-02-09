using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("BANCOS")]
    public class Banco
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdBanco { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(250)]
        public string Enlace { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxCredito { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal HonorarioAbogado { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Comision { get; set; }

        [Required]
        [StringLength(30)]
        public string TipoCambio { get; set; }

        public string? Logo { get; set; }
        public List<EscenarioTasaInteres> EscenariosTasaInteres { get; set; } = new();
        public List<EndeudamientoMaximo> EndeudamientoMaximos { get; set; } = new();
        public List<SeguroBanco> SeguroBancos { get; set; } = new();
    }

}
