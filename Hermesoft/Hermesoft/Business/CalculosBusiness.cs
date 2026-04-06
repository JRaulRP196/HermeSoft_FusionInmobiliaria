using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Models.Banco;
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
        private readonly EscenarioTasaInteresRepository _escenarioTasaInteresRepository;
        private readonly TipoCambioRepository _tipoCambioRepository;

        public CalculosBusiness(
            LoteRepository loteRepository,
            Configuracion configuracion,
            BancoRepository bancoRepository,
            EscenarioTasaInteresRepository escenarioTasaInteresRepository,
            TipoCambioRepository tipoCambioRepository)
        {
            _loteRepository = loteRepository;
            _configuracion = configuracion;
            _bancoRepository = bancoRepository;
            _escenarioTasaInteresRepository = escenarioTasaInteresRepository;
            _tipoCambioRepository = tipoCambioRepository;
        }

        #region Utilidades

        public async Task<IEnumerable<DesglosesPrimas>> CalcularPrima(string codigoLote, decimal porcentajePrima, DateTime fechaFinal)
        {
            return await CalcularPrima(codigoLote, porcentajePrima, fechaFinal, null);
        }

        public async Task<IEnumerable<DesglosesPrimas>> CalcularPrima(string codigoLote, decimal porcentajePrima, DateTime fechaFinal, decimal? porcentajeDescuento)
        {
            var lote = await _loteRepository.Obtener(codigoLote);
            VerificarDatosPrima(lote, porcentajePrima, fechaFinal);

            DateTime inicio = DateTime.Today;

            decimal precioParaCalculo = lote.PrecioVenta;

            if (porcentajeDescuento.HasValue && porcentajeDescuento.Value > 0)
            {
                if (porcentajeDescuento.Value > 100)
                    throw new Exception("El porcentaje de descuento no puede ser mayor a 100.");

                precioParaCalculo = lote.PrecioVenta * (1 - (porcentajeDescuento.Value / 100m));
            }

            var prima = new Primas
            {
                Porcentaje = porcentajePrima,
                FechaInicio = inicio,
                FechaCierre = fechaFinal,
                Total = precioParaCalculo * (porcentajePrima / 100m)
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

        public async Task<List<CuotaBancaria>> CalcularCuotasBancaria(int idEscenario, string codigoLote)
        {
            EscenarioTasaInteres escenario = await _escenarioTasaInteresRepository.Obtener(idEscenario);
            if (escenario == null) throw new Exception("Escenario invalido");

            Lote lote = await _loteRepository.Obtener(codigoLote);
            if (lote == null) throw new Exception("Lote no válido");

            List<CuotaBancaria> cuotas = new List<CuotaBancaria>();
            foreach(PlazosEscenarios plazo in escenario.PlazosEscenarios)
            {
                double tasaAnual = (plazo.Indicador.PorcSeguro ?? 0)
                     + (plazo.PorcAdicional ?? 0);
                double tasaMensual = (tasaAnual / 100) / 12;
                double P = (double)lote.PrecioVenta;

                double cuotaMensual = P * (tasaMensual * Math.Pow(1 + tasaMensual, plazo.Plazo))
                                        / (Math.Pow(1 + tasaMensual, plazo.Plazo) - 1);

                CuotaBancaria cuota = new CuotaBancaria
                {
                    Plazo = plazo.Plazo,
                    TasaInteres = tasaAnual,
                    MontoMensual = cuotaMensual
                };
                cuotas.Add(cuota);
            }
            return cuotas;
        }

        public async Task<decimal> CalcularGastoFormalizacion(decimal seguroVida, decimal seguroDesempleo, decimal honorarioAbogados, 
            decimal comisionBancaria, string codLote)
        {

            if (seguroVida < 0 || seguroDesempleo < 0 || honorarioAbogados < 0 || comisionBancaria < 0 || codLote == null) 
                throw new Exception("Se necesitan datos correctos");

            var lote = await _loteRepository.Obtener(codLote);
            var montoHonorarioAbogados = lote.PrecioVenta * (honorarioAbogados * 0.01m);
            montoHonorarioAbogados = montoHonorarioAbogados + (montoHonorarioAbogados * (decimal.Parse(_configuracion.ObtenerValor("IVA")) * 0.01m));
            // var porcentajeExtra = seguroVida + seguroDesempleo + comisionBancaria + decimal.Parse(_configuracion.ObtenerValor("TimbreFiscal")); //Q Borrar debugging
            var timbreFiscal = decimal.Parse(_configuracion.ObtenerValor("TimbreFiscal"), CultureInfo.InvariantCulture);
            var porcentajeExtra = seguroVida + seguroDesempleo + comisionBancaria + timbreFiscal;
            var montoExtra = lote.PrecioVenta * (porcentajeExtra * 0.01m);
            return montoHonorarioAbogados + montoExtra;
        }

        public async Task<decimal> CalcularIngresoNetoFamiliar(int idBanco, int idEndeudamiento, decimal cuotaMensual)
        {
            var banco = await _bancoRepository.ObtenerPorId(idBanco);
            if (banco == null) throw new Exception("Banco inválido");
            var endeudamientoMaximo = banco.EndeudamientoMaximos.FirstOrDefault(e => e.IdEndeudamiento == idEndeudamiento);
            if (endeudamientoMaximo == null) throw new Exception("Endeudamiento máximo inválido");
            if (cuotaMensual <= 0) throw new Exception("Cuota mensual no calculada");

            return cuotaMensual / (endeudamientoMaximo.PorcEndeudamiento * 0.01m);
        }

        public async Task<double?> ObtenerCambioDelDolar()
        {
            var cambio = await _tipoCambioRepository.Obtener();
            if (cambio == null || cambio.Cambio == null) throw new Exception("Ocurrio un error al obtener el cambio del dolar");
            return cambio.Cambio;
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

        public decimal CalcularTotalPrima(IEnumerable<DesglosesPrimas> desgloses)
        {
            decimal total = 0;
            foreach(var desglose in desgloses)
            {
                total += desglose.Monto;
            }
            return total;
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
