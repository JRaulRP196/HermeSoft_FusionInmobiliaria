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

        public async Task<PagoCondominioViewModel> PagosPorCondominio(string? condominio, DateTime? fechaInicio, DateTime? fechaFinal)
        {
            return await _ventaBusiness.ObtenerPagosPorCondominio(condominio, fechaInicio, fechaFinal);
        }

        #endregion
    }
}
