using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models.Usuarios
{
    public class RecuperacionPassword
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRecuperacion {  get; set; }
        [Required]
        public int IdUsuario { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public DateTime FechaExpiracion { get; set; }
        [Required]
        public bool Usado {  get; set; }
        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }

    }
}
