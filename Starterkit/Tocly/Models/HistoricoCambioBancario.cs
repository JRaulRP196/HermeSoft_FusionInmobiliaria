using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("HISTORICO_CAMBIOS_BANCARIOS")]
    public class HistoricoCambioBancario
    {
        [Key]
        public int idHistorico { get; set; }

        public int idBanco { get; set; }

        public DateTime fechaCambio { get; set; }

        public string usuarioNombre { get; set; } = "";
        public string usuarioCorreo { get; set; } = "";

        public string tablaAfectada { get; set; } = "";

        public string informacionAnterior { get; set; } = "";
        public string informacionNueva { get; set; } = "";
    }
}
