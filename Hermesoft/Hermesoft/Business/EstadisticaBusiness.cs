using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Models.ViewModels;

namespace HermeSoft_Fusion.Business
{
    public class EstadisticaBusiness
    {

        private readonly VentaBusiness _ventaBusiness;
        private readonly LoteBusiness _loteBusiness;

        public EstadisticaBusiness(VentaBusiness ventaBusiness, LoteBusiness loteBusiness)
        {
            _ventaBusiness = ventaBusiness;
            _loteBusiness = loteBusiness;
        }

        #region Utilidades

        public async Task<PagoCondominioViewModel> PagosPorCondominio(string condominio, DateTime fechaInicio, DateTime fechaFinal)
        {
            var lotes = await _loteBusiness.ObtenerPorCondominio(condominio);
            List<Venta> ventas = await _ventaBusiness.Obtener();

            ventas = ventas.Where(v => lotes.Any(l => l.Codigo == v.CodLote)).ToList();

            if (fechaInicio != DateTime.MinValue && fechaFinal != DateTime.MinValue)
            {
                if (fechaInicio <= fechaFinal)
                {
                    ventas = ventas.Where(v => v.FechaDeRegistro >= fechaInicio && v.FechaDeRegistro <= fechaFinal).ToList();
                }
                else
                {
                    throw new Exception("La fecha de inicio debe ser menor o igual a la fecha final.");
                }
            }

            int pagosPendientes = 0;
            int pagosPagados = 0;
            int pagosAtrasados = 0;

            if (ventas.Any())
            {
                pagosPendientes = ventas
                    .Where(v => v.Prima != null && v.Prima.DesglosesPrimas != null)
                    .SelectMany(v => v.Prima.DesglosesPrimas.Where(dp => dp.Estado == "Pendiente"))
                    .Count();

                pagosPagados = ventas
                    .Where(v => v.Prima != null && v.Prima.DesglosesPrimas != null)
                    .SelectMany(v => v.Prima.DesglosesPrimas.Where(dp => dp.Estado == "Pagado"))
                    .Count();

                pagosAtrasados = ventas
                    .Where(v => v.Prima != null && v.Prima.DesglosesPrimas != null)
                    .SelectMany(v => v.Prima.DesglosesPrimas.Where(dp => dp.Estado == "Atrasado"))
                    .Count();
            }

            return new PagoCondominioViewModel
            {
                Pendientes = pagosPendientes,
                Pagados = pagosPagados,
                Atrasados = pagosAtrasados
            };
        }

     
        public async Task<List<PagoCondominioViewModel>> PagosTodos(List<dynamic> condominios, DateTime fechaInicio, DateTime fechaFinal)
        {
            var resultado = new List<PagoCondominioViewModel>();

            foreach (var c in condominios)
            {
                var datos = await PagosPorCondominio(c.id.ToString(), fechaInicio, fechaFinal);

                if (datos.Pagados > 0 || datos.Pendientes > 0 || datos.Atrasados > 0)
                {
                    resultado.Add(new PagoCondominioViewModel
                    {
                        Condominio = c.nombre,
                        Pagados = datos.Pagados,
                        Pendientes = datos.Pendientes,
                        Atrasados = datos.Atrasados
                    });
                }
            }

            return resultado;
        }

        #endregion
    }
}