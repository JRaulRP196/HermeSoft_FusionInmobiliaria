using HermeSoft_Fusion.Models.Usuarios;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    public class Venta
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NumContrato { get; set; }

        [Required]
        [StringLength(70)]
        public string CorreoCliente { get; set; }

        [Required]
        public decimal GastoFormalizacion { get; set; }

        [Required]
        [StringLength(30)]
        public string CodLote { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; }

        [StringLength(300)]
        public string? MotivoNulidad { get; set; }

        [Required]
        public DateTime FechaDeRegistro { get; set; }

        [Required]
        public int? IdPrima { get; set; }

        [Required]
        public int IdBanco { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [ForeignKey("IdPrima")]
        public Primas Prima { get; set; }

        [ForeignKey("IdBanco")]
        public Models.Banco.Banco Banco { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }

    }
}
