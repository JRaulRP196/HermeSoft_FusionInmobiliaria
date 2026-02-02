using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("TIPOS_ASALARIADOS")]
    public class TipoAsalariado
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTipoAsalariado { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
    }
}
