using HermeSoft_Fusion.Models;
using HermeSoft_Fusion.Models.Usuarios;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace HermeSoft_Fusion.Business.Usuarios
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoAsync(
            string destino,
            string asunto,
            string mensaje,
            byte[]? archivo = null,
            string? nombreArchivo = null)
        {
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(_config["EmailSettings:Email"]));
            email.To.Add(MailboxAddress.Parse(destino));
            email.Subject = asunto;

            var builder = new BodyBuilder
            {
                HtmlBody = mensaje
            };

            if (archivo != null && nombreArchivo != null)
            {
                builder.Attachments.Add(nombreArchivo, archivo, ContentType.Parse("application/pdf"));
            }

            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _config["EmailSettings:Host"],
                int.Parse(_config["EmailSettings:Port"]),
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _config["EmailSettings:Email"],
                _config["EmailSettings:Password"]
            );

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public string GenerarMensajeRecordatorio(DesglosesPrimas desglose)
        {
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "RecordatorioPago.html");
            string html = System.IO.File.ReadAllText(ruta);
            html = html.Replace("{{correoAsesor}}", desglose.Prima.Venta.Usuario.Correo);

            html = html.Replace("{{fechaCobro}}", desglose.FechaCobro.ToString("dd/MM/yyyy"));
            html = html.Replace("{{monto}}", desglose.Monto.ToString());
            var link = _config.GetSection("Url").Value + $"Recordatorio?idDesglose={desglose.IdDesglosePrima}";
            html = html.Replace("{{linkConfirmacion}}", link);
            return html;
        }

        public string GenerarMensajeConfirmacion (DesglosesPrimas desglose)
        {
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "ConfirmacionPagoAsesor.html");
            string html = System.IO.File.ReadAllText(ruta);
            html = html.Replace("{{correoCliente}}", desglose.Prima.Venta.CorreoCliente);

            html = html.Replace("{{fechaCobro}}", desglose.FechaCobro.ToString("dd/MM/yyyy"));
            html = html.Replace("{{monto}}", desglose.Monto.ToString());
            var link = _config.GetSection("Url").Value + $"Recordatorio/ConfirmacionAsesor?idDesglose={desglose.IdDesglosePrima}";
            html = html.Replace("{{linkConfirmacion}}", link);
            return html;
        }

        public string GenerarMensajePrimaCompleta( Venta venta)
        {
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "NotificacionPrimaCompleta.html");
            string html = System.IO.File.ReadAllText(ruta);
            html = html.Replace("{{numContrato}}", venta.NumContrato.ToString());
            return html;
        }

        public string GenerarMensajePassword(string password)
        {
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "Password.html");
            string html = System.IO.File.ReadAllText(ruta);
            html = html.Replace("{{CONTRASENA_TEMPORAL}}", password);
            return html;
        }

        public string GenerarMensajeRecuperacion(RecuperacionPassword recuperacion)
        {
            string enlace = _config["Url"] + $"Auth/NuevoPassword?token={recuperacion.Token}";
            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "email", "CambiarPassword.html");
            string html = System.IO.File.ReadAllText(ruta);
            html = html.Replace("{{ENLACE_RECUPERACION}}", enlace);
            return html;
        }

    }
}
