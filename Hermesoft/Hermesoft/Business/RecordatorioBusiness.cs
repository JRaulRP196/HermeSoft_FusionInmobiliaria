using HermeSoft_Fusion.Business.Usuarios;
using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;

namespace HermeSoft_Fusion.Business
{
    public class RecordatorioBusiness
    {

        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public RecordatorioBusiness(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
