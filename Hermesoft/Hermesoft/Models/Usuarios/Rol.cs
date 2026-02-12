using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models.Usuarios
{
    public class Rol
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRol { get; set; }

        [Required]
        [StringLength(30)]
        public string Nombre {  get; set; }

        public List<Usuario> Usuarios { get; set; } = new ();
    }
}
