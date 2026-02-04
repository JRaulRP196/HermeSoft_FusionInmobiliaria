using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class CalculosBusiness
    {
        private LoteRepository _loteRepository;

     
        private IndicadorBancarioRepository _indicadorRepo;

        public CalculosBusiness(LoteRepository loteRepository, IndicadorBancarioRepository indicadorRepo)
        {
            _loteRepository = loteRepository;
            _indicadorRepo = indicadorRepo;
        }

        #region Utilidades

        public async Task<IEnumerable<DesglosesPrimas>> CalcularPrima(string codigoLote, decimal porcentajePrima, DateTime fechaFinal)
        {
            var lote = await _loteRepository.Obtener(codigoLote);
            VerificarDatosPrima(lote, porcentajePrima, fechaFinal);

            DateTime inicio = DateTime.Today;

            var prima = new Primas
            {
                Porcentaje = porcentajePrima,
                FechaInicio = inicio,
                FechaCierre = fechaFinal,
                Total = lote.PrecioVenta * (porcentajePrima / 100m)
            };

            int meses = (prima.FechaCierre.Year - prima.FechaInicio.Year) * 12
                      + (prima.FechaCierre.Month - prima.FechaInicio.Month);

            if (prima.FechaCierre.Day < prima.FechaInicio.Day)
                meses--;

            if (meses <= 0)
                throw new Exception("El rango de fechas no genera meses válidos");

            decimal montoMensual = prima.Total / meses;

            var desglosePrima = new List<DesglosesPrimas>();

            for (int i = 1; i <= meses; i++)
            {
                desglosePrima.Add(new DesglosesPrimas
                {
                    FechaCobro = prima.FechaInicio.AddMonths(i),
                    Monto = montoMensual,
                    Estado = "Pendiente",
                    Prima = prima
                });
            }

            return desglosePrima;
        }

        #endregion

        #region Helpers

        public void VerificarDatosPrima(Lote lote, decimal porcentajePrima, DateTime fechaFinal)
        {
            if (lote == null)
                throw new Exception("Lote inválido");

            if (porcentajePrima <= 0)
                throw new Exception("El porcentaje debe ser mayor a 0");

            if (fechaFinal == DateTime.MinValue)
                throw new Exception("Fecha inválida");

            DateTime hoy = DateTime.Today;

            if (fechaFinal <= hoy)
                throw new Exception("La fecha debe ser mayor a hoy");

            if (fechaFinal.Year == hoy.Year && fechaFinal.Month == hoy.Month)
                throw new Exception("La fecha debe estar en un mes distinto al actual");
        }

        #endregion


        


        public async Task<decimal> CalcularPorcentajeFormalizacion(string codigoLote)
        {
            if (string.IsNullOrWhiteSpace(codigoLote))
                throw new Exception("Debe seleccionar lote.");

            var lote = await _loteRepository.Obtener(codigoLote);
            if (lote == null)
                throw new Exception("Lote inválido.");

            
            var indicador = await _indicadorRepo.ObtenerPorNombre("SOFR");

            if (indicador == null)
                throw new Exception("No existe el indicador en BD. Primero debe actualizarse desde BCCR.");

            return indicador.PorcSeguro;
        }

        
        public async Task<decimal> CalcularMontoFormalizacion(string codigoLote)
        {
            var lote = await _loteRepository.Obtener(codigoLote);
            if (lote == null)
                throw new Exception("Lote inválido.");

            var porcentaje = await CalcularPorcentajeFormalizacion(codigoLote);
            return Math.Round((porcentaje / 100m) * lote.PrecioVenta, 2);
        }
    }
}
