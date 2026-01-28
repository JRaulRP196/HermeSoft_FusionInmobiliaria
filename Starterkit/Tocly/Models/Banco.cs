using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("BANCOS")]
    public class Banco
    {
        [Key]
        public int idBanco { get; set; }

        public string nombre { get; set; } = "";
        public string enlace { get; set; } = "";

        public decimal maxCredito { get; set; }
        public decimal honorarioAbogado { get; set; }
        public decimal comision { get; set; }

        public string tipoCambio { get; set; } = "CRC";

        // Si NO existe columna logo en BD_FUSION, déjalo comentado
        public string? logo { get; set; }
    }
}
