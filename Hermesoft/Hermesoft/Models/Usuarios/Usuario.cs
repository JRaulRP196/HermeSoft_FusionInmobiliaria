using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models.Usuarios
{
    public class Usuario
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(50)]
        public string Apellido1 { get; set; }
        [Required]
        [StringLength(50)]
        public string Apellido2 { get; set; }
        [Required]
        [EmailAddress]
        [RegularExpression(@"(?i)^[^@\s]+@fusioninmobiliariacr\.com$", ErrorMessage = "El correo debe pertenecer al dominio @fusioninmobiliariacr.com")]
        [StringLength(70)]
        public string Correo { get; set; }
        [StringLength(300)]
        public string? Password { get; set; }
        [Required]
        public bool Estado { get; set; }
        public int IdRol { get; set; }
        [ForeignKey("IdRol")]
        public Rol? Rol { get; set; }
        public List<RecuperacionPassword> Recuperaciones { get; set; } = new();
        public List<Venta> Ventas { get; set; } = new();
    }
}
