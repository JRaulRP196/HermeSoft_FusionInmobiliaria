using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class VentaBusiness
    {

        private readonly VentaRepository _ventaRepository;
        private readonly AppDbContext _context;
        private readonly PrimaBusiness _primaBusiness;
        private readonly LoteBusiness _loteBusiness;

        public VentaBusiness(VentaRepository ventaRepository, AppDbContext context, PrimaBusiness primaBusiness, LoteBusiness loteBusiness)
        {
            _ventaRepository = ventaRepository;
            _context = context;
            _primaBusiness = primaBusiness;
            _loteBusiness = loteBusiness;
        }

        #region Utilidades

        public async Task<Venta> Agregar(Venta venta)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                venta.FechaDeRegistro = DateTime.Now;
                venta.Estado = "EN PROCESO";
                var prima = await _primaBusiness.Obtener(venta.CorreoCliente);
                if(prima == null)
                {
                    throw new Exception($"Debe registrar una prima para el cliente {venta.CorreoCliente}");
                }
                venta.IdPrima = prima.IdPrima;
                await _ventaRepository.Agregar(venta);

                var lote = await _loteBusiness.Obtener(venta.CodLote);
                lote.Estado = "En Venta";
                await _loteBusiness.Editar(lote);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return venta;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        #endregion

    }
}
