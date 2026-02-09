using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("HISTORICO_CAMBIOS_BANCARIOS")]
    public class HistoricoCambiosBancarios
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdHistorico { get; set; }

        [Required]
        [Column("fechaCambio", TypeName = "date")]
        public DateTime FechaCambio { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("usuarioNombre")]
        public string UsuarioNombre { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("usuarioCorreo")]
        public string UsuarioCorreo { get; set; }

        [Required]
        [MaxLength(60)]
        [Column("tablaAfectada")]
        public string TablaAfectada { get; set; }

        [Required]
        [Column("informacionAnterior", TypeName = "longtext")]
        public string InformacionAnterior { get; set; }

        [Required]
        [Column("informacionNueva", TypeName = "longtext")]
        public string InformacionNueva { get; set; }
    }
}
