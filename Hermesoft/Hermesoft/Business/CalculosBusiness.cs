using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Models.Servicios;
using HermeSoft_Fusion.Repository;
using HermeSoft_Fusion.Repository.Servicios;
using System.Globalization;

namespace HermeSoft_Fusion.Business
{
    public class CalculosBusiness
    {
        private readonly LoteRepository _loteRepository;
        private readonly Configuracion _configuracion;
        private readonly BancoRepository _bancoRepository;

        public CalculosBusiness(
            LoteRepository loteRepository,
            Configuracion configuracion,
            BancoRepository bancoRepository)
        {
            _loteRepository = loteRepository;
            _configuracion = configuracion;
            _bancoRepository = bancoRepository;
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

        public decimal ObtenerTimbreFiscal()
        {
            try
            {
                string? valor = _configuracion.ObtenerValor("TimbreFiscal");

                if (string.IsNullOrWhiteSpace(valor))
                    return -1;

                // 1) Cultura actual
                if (decimal.TryParse(valor, NumberStyles.Any, CultureInfo.CurrentCulture, out var timbreLocal))
                    return timbreLocal;

                // 2) Invariante (punto decimal)
                if (decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out var timbreInv))
                    return timbreInv;

                // 3) Normalizar coma/punto
                var normalizado = valor.Replace(",", ".");
                if (decimal.TryParse(normalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out var timbreNorm))
                    return timbreNorm;

                return -1;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<object> CalcularGastoFormalizacionDesdeBD(string codigoLote, int idBanco)
        {
            var lote = await _loteRepository.Obtener(codigoLote);
            if (lote == null) throw new Exception("Lote inválido");

            var banco = await _bancoRepository.ObtenerPorId(idBanco);
            if (banco == null) throw new Exception("Banco inválido");

            // % desde BD
            decimal porcComision = banco.Comision;
            decimal porcAbogadosBase = banco.HonorarioAbogado;

            // Honorarios + IVA 13%
            decimal porcAbogadosConIva = porcAbogadosBase * 1.13m;

            // Seguros desde BD (SEGUROS_BANCOS + SEGUROS)
            decimal porcVida = 0m;
            decimal porcDesempleo = 0m;

            if (banco.SeguroBancos != null)
            {
                foreach (var sb in banco.SeguroBancos)
                {
                    var nombre = (sb?.Seguro?.Nombre ?? "").ToLower();

                    if (nombre.Contains("vida"))
                        porcVida = sb.PorcSeguro;

                    if (nombre.Contains("desempleo"))
                        porcDesempleo = sb.PorcSeguro;
                }
            }

            // Timbre fiscal: si -1 => para cálculo usar 0 (pero se devuelve -1 para vista)
            decimal timbre = ObtenerTimbreFiscal();
            decimal timbreParaCalculo = timbre < 0 ? 0 : timbre;

            decimal totalPorcentaje = porcVida + porcDesempleo + porcAbogadosConIva + porcComision + timbreParaCalculo;

            // monto con PrecioVenta del lote
            decimal gastoFormalizacion = lote.PrecioVenta * (totalPorcentaje / 100m);

            return new
            {
                porcVida,
                porcDesempleo,
                porcAbogados = Math.Round(porcAbogadosConIva, 4),
                porcComision,
                timbreFiscal = timbre, // puede venir -1
                totalPorcentaje = Math.Round(totalPorcentaje, 4),
                gastoFormalizacion = Math.Round(gastoFormalizacion, 2),
                precioLote = lote.PrecioVenta
            };
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
    }
}
