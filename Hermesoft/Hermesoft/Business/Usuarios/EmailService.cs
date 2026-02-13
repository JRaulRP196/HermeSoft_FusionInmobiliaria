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

        public async Task EnviarCorreoAsync(string destino, string asunto, string mensaje)
        {
            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(_config["EmailSettings:Email"]));
            email.To.Add(MailboxAddress.Parse(destino));
            email.Subject = asunto;

            email.Body = new TextPart("html")
            {
                Text = mensaje
            };

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


        public string GenerarMensajePassword(string password)
        {
            string plantillaHtml = @"
                                <!DOCTYPE html>
                                <html lang='es'>
                                <head>
                                    <meta charset='UTF-8'>
                                </head>
                                <body style='margin:0; padding:0; background:#f4f4f4; font-family:Arial, sans-serif;'>

                                <table width='100%' cellpadding='0' cellspacing='0' style='padding:30px 0;'>
                                    <tr>
                                        <td align='center'>
                                            <table width='500' cellpadding='0' cellspacing='0' 
                                                   style='background:#ffffff; border-radius:8px; padding:30px; text-align:center;'>

                                                <h2 style='margin-top:0; color:#333;'>Contraseña Temporal</h2>

                                                <p style='color:#555; font-size:15px;'>
                                                    Hemos generado una contraseña temporal para tu cuenta:
                                                </p>

                                                <p style='font-size:20px; font-weight:bold; 
                                                          background:#f0f0f0; 
                                                          padding:12px; 
                                                          border-radius:6px; 
                                                          letter-spacing:2px;'>
                                                    {{CONTRASENA_TEMPORAL}}
                                                </p>

                                                <p style='color:#777; font-size:14px;'>
                                                    Por seguridad, cámbiala después de iniciar sesión.
                                                </p>

                                            </table>
                                        </td>
                                    </tr>
                                </table>

                                </body>
                                </html>";
            string htmlFinal = plantillaHtml.Replace("{{CONTRASENA_TEMPORAL}}", password);
            return htmlFinal;
        }

        public string GenerarMensajeRecuperacion(RecuperacionPassword recuperacion)
        {
            string enlace = _config["EnlacePagina"] + $"Auth/NuevoPassword?token={recuperacion.Token}";
            string plantillaHtml = @"
                                <!DOCTYPE html>
                                <html lang='es'>
                                <head>
                                    <meta charset='UTF-8'>
                                </head>
                                <body style='margin:0; padding:0; background:#f4f4f4; font-family:Arial, sans-serif;'>

                                <table width='100%' cellpadding='0' cellspacing='0' style='padding:30px 0;'>
                                    <tr>
                                        <td align='center'>
                                            <table width='500' cellpadding='0' cellspacing='0' 
                                                   style='background:#ffffff; border-radius:8px; padding:30px; text-align:center;'>

                                                <h2 style='margin-top:0; color:#333;'>Recuperación de contraseña</h2>

                                                <p style='color:#555; font-size:15px;'>
                                                    Hemos recibido una solicitud para restablecer tu contraseña.
                                                </p>

                                                <p style='color:#555; font-size:15px;'>
                                                    Haz clic en el siguiente botón para continuar:
                                                </p>

                                                <a href='{{ENLACE_RECUPERACION}}' 
                                                   style='display:inline-block;
                                                          padding:12px 20px;
                                                          background:#007bff;
                                                          color:#ffffff;
                                                          text-decoration:none;
                                                          border-radius:6px;
                                                          font-size:15px;
                                                          font-weight:bold;
                                                          margin-top:10px;'>
                                                    Restablecer contraseña
                                                </a>

                                                <p style='color:#777; font-size:13px; margin-top:20px;'>
                                                    Este enlace expirará en 5 minutos.
                                                </p>

                                                <p style='color:#999; font-size:12px; margin-top:15px;'>
                                                    Si no solicitaste este cambio, puedes ignorar este correo.
                                                </p>

                                            </table>
                                        </td>
                                    </tr>
                                </table>

                                </body>
                                </html>";

            string htmlFinal = plantillaHtml.Replace("{{ENLACE_RECUPERACION}}", enlace);
            return htmlFinal;
        }

    }
}
