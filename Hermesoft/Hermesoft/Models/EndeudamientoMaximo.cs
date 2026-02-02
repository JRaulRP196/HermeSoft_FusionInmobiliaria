using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("ENDEUDAMIENTOS_MAXIMOS")]
    public class EndeudamientoMaximo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdEndeudamiento { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PorcEndeudamiento { get; set; }

        public int IdBanco { get; set; }
        [ForeignKey("IdBanco")]
        public Banco Banco { get; set; }

        public int IdTipoAsalariado { get; set; }
        [ForeignKey("IdTipoAsalariado")]
        public TipoAsalariado TipoAsalariado { get; set; }
    }
}
