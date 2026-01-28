using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("ENDEUDAMIENTOS_MAXIMOS")]
    public class EndeudamientoMaximo
    {
        [Key]
        public int idEndeudamiento { get; set; }

        public decimal porcEndeudamiento { get; set; }

        public int idBanco { get; set; }
        public int idTipoAsalariado { get; set; }
    }
}
