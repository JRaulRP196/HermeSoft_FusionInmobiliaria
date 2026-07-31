using HermeSoft_Fusion.Business.Usuarios;
using HermeSoft_Fusion.Models;

namespace HermeSoft_Fusion.Repository.Servicios
{
    public class Job
    {
        private readonly EmailService _emailService;
        private readonly VentaRepository _ventaRepository;

        public Job(EmailService emailService, VentaRepository ventaRepository)
        {
            _emailService = emailService;
            _ventaRepository = ventaRepository;
        }

        public async Task EnviarRecordatorios()
        {
            var desgloses = await _ventaRepository.ObtenerPagosProximosVencer();
            foreach (var desglose in desgloses)
            {
                    string mensaje = _emailService.GenerarMensajeRecordatorio(desglose);
                    await _emailService.EnviarCorreoAsync(desglose.Prima.Venta.CorreoCliente, "Recordatorio de Pago", mensaje);
            }
        }

    }
}
