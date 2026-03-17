using HermeSoft_Fusion.Business.Usuarios;

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
            var ventas = await _ventaRepository.ObtenerPendientes();
            foreach (var venta in ventas)
            {
                var desglose = venta.Prima.DesglosesPrimas.FirstOrDefault(d => d.Estado == "Pendiente");
                if (desglose != null && (desglose.FechaCobro - DateTime.Today).TotalDays <= 2)
                {
                    string mensaje = _emailService.GenerarMensajeRecordatorio(venta);
                    await _emailService.EnviarCorreoAsync(venta.CorreoCliente, "Recordatorio de Pago", mensaje);
                }
            }
        }

    }
}
