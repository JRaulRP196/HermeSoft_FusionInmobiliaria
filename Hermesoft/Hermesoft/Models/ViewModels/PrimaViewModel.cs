namespace HermeSoft_Fusion.Models.ViewModels
{
    public class PrimaViewModel
    {
        public string Lote { get; set; } = string.Empty;
        public string? Cliente { get; set; }

        public decimal Porcentaje { get; set; }
        public decimal PorcentajeDescuento { get; set; }
        public DateTime FechaCierre { get; set; } = DateTime.Today;

        public string? CorreoCliente { get; set; }

        public List<DesglosesPrimas> DesglosesSinDescuento { get; set; } = new();
        public List<DesglosesPrimas> DesglosesConDescuento { get; set; } = new();

        public bool AplicarDescuento => PorcentajeDescuento > 0;
        public bool TieneDesglose => DesglosesSinDescuento.Any() || DesglosesConDescuento.Any();

        public string? MensajeErrorPrima { get; set; }
        public string? MensajeExitoPrima { get; set; }
    }
}
