using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("TIPOS_ASALARIADOS")]
    public class TipoAsalariado
    {
        [Key]
        public int idTipoAsalariado { get; set; }
        public string nombre { get; set; } = "";
    }
}
