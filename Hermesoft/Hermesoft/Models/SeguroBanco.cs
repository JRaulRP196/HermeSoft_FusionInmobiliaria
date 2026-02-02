using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("SEGUROS_BANCOS")]
    public class SeguroBanco
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdSeguroBanco { get; set; }

        public int IdBanco { get; set; }
        [ForeignKey("IdBanco")]
        public Banco Banco { get; set; }

        public int IdSeguro { get; set; }
        [ForeignKey("IdSeguro")]
        public Seguro Seguro { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PorcSeguro { get; set; }
    }
}
