using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HermeSoft_Fusion.Models
{
    [Table("HISTORICO_CAMBIOS_BANCARIOS")]
    public class HistoricoCambioBancario
    {
        [Key]
        [Column("idHistorico")]
        public int IdHistorico { get; set; }

        [Column("idBanco")]
        public int IdBanco { get; set; }

        [Column("fechaCambio")]
        public DateTime FechaCambio { get; set; } = DateTime.Now;

        [Column("usuarioNombre")]
        public string UsuarioNombre { get; set; } = "";

        [Column("usuarioCorreo")]
        public string UsuarioCorreo { get; set; } = "";

        [Column("tablaAfectada")]
        public string TablaAfectada { get; set; } = "";

        [Column("informacionAnterior")]
        public string InformacionAnterior { get; set; } = "";

        [Column("informacionNueva")]
        public string InformacionNueva { get; set; } = "";
    }
}
