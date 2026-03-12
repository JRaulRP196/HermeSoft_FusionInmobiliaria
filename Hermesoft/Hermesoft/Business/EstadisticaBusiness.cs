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

        public async Task<PagoCondominioViewModel> PagosPorCondominio(string condominio)
        {
            var lotes = await _loteBusiness.ObtenerPorCondominio(condominio);
            List<Venta> ventas = await _ventaBusiness.Obtener();
            ventas = ventas.Where(v => lotes.Any(l => l.Codigo == v.CodLote)).ToList();
            int pagosPendientes = 0;
            int pagosPagados = 0;
            int pagosAtrasados = 0;

            if (ventas.Any())
            {
                pagosPendientes = ventas.SelectMany(v => v.Prima.DesglosesPrimas.Where(dp => dp.Estado == "Pendiente")).ToList().Count();
                pagosPagados = ventas.SelectMany(v => v.Prima.DesglosesPrimas.Where(dp => dp.Estado == "Pagado")).ToList().Count();
                pagosAtrasados = ventas.SelectMany(v => v.Prima.DesglosesPrimas.Where(dp => dp.Estado == "Atrasado")).ToList().Count();
            }

            return new PagoCondominioViewModel
            {
                Pendientes = pagosPendientes,
                Pagados = pagosPagados,
                Atrasados = pagosAtrasados
            };
        }

        #endregion
    }
}
