using HermeSoft_Fusion.Business.Usuarios;
using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Repository;

namespace HermeSoft_Fusion.Business
{
    public class RecordatorioBusiness
    {

        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly VentaRepository _ventaRepository;
        private readonly LoteBusiness _loteBusiness;

        public RecordatorioBusiness(AppDbContext context, EmailService emailService, VentaRepository ventaRepository
            , LoteBusiness loteBusiness)
        {
            _context = context;
            _emailService = emailService;
            _ventaRepository = ventaRepository;
            _loteBusiness = loteBusiness;
        }

        public async Task<bool> ConfirmarPago(DesglosesPrimas desglose)
        {
            try
            {
                string mensaje = _emailService.GenerarMensajeConfirmacion(desglose);
                await _emailService.EnviarCorreoAsync(desglose.Prima.Venta.Usuario.Correo, $"Confirmación de Pago del Cliente {desglose.Prima.Venta.CorreoCliente}", mensaje);
                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }
        }

        public async Task ActualizarDesglose(DesglosesPrimas desglose)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (desglose.FechaPagado != null)
                    return;
                desglose.Estado = "Terminado";
                desglose.FechaPagado = DateTime.Now;
                var venta = await _ventaRepository.Obtener(desglose.Prima.Venta.NumContrato);
                if(venta.Prima.DesglosesPrimas.Where(d => d.Estado == "Pendiente").ToList().Count() == 0)
                {
                    var lote = await _loteBusiness.Obtener(venta.CodLote);
                    lote.Estado = "Vendido";
                    await _loteBusiness.Editar(lote);
                    venta.Estado = "TERMINADO";
                    string mensaje = _emailService.GenerarMensajePrimaCompleta(venta);
                    await _emailService.EnviarCorreoAsync(venta.Usuario.Correo,"Cobro de comisión disponible", mensaje);
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw new Exception("Ocurrio un error a la hora de actualziar el pago");
            }
        } 
    }
}
