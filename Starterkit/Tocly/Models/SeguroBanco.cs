using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("SEGUROS_BANCOS")]
    public class SeguroBanco
    {
        [Key]
        public int idSeguroBanco { get; set; }

        public int idBanco { get; set; }
        public int idSeguro { get; set; }
    }
}
