using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models.Usuarios;
using HermeSoft_Fusion.Repository.Usuarios;

namespace HermeSoft_Fusion.Business.Usuarios
{
    public class UsuarioBusiness
    {

        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly EmailService _emailService;
        private readonly TokenService _tokenService;
        private readonly RecuperacionPasswordRepository _recuperacionPasswordRepository;

        public UsuarioBusiness(AppDbContext context, PasswordService passwordService, UsuarioRepository usuarioRepository,
            EmailService emailService, TokenService tokenService, RecuperacionPasswordRepository recuperacionPasswordRepository)
        {
            _context = context;
            _passwordService = passwordService;
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
            _tokenService = tokenService;
            _recuperacionPasswordRepository = recuperacionPasswordRepository;
        }

        public async Task<Usuario> Agregar(Usuario usuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await VerificarExistenciaUsuario(usuario))
                {
                    usuario.IdUsuario = -1;
                    return usuario;
                }
                string mensajeCorreo = _emailService.GenerarMensajePassword(usuario.Password);
                usuario.Password = _passwordService.HashPassword(usuario, usuario.Password);
                await _usuarioRepository.Agregar(usuario);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                await _emailService.EnviarCorreoAsync(usuario.Correo, "Contraseña temporal", mensajeCorreo);
                return usuario;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Usuario> SolicitudCambio(Usuario usuario, string correoCookie)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!await VerificarExistenciaUsuario(usuario) || usuario.Correo != correoCookie)
                {
                    usuario.IdUsuario = -1;
                    return usuario;
                }
                Usuario user = await _usuarioRepository.Obtener(usuario.Correo);
                RecuperacionPassword recuperacion = new RecuperacionPassword
                {
                    IdUsuario = user.IdUsuario,
                    FechaExpiracion = DateTime.Now.AddMinutes(5),
                    Usado = false,
                    Token = _tokenService.GenerarToken()
                };
                await _recuperacionPasswordRepository.Agregar(recuperacion);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                string mensaje = _emailService.GenerarMensajeRecuperacion(recuperacion);
                await _emailService.EnviarCorreoAsync(usuario.Correo, "Cambio de contraseña", mensaje);
                return usuario;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Usuario> CambiarPassword(Usuario usuario, string token)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                RecuperacionPassword recuperacion = await _recuperacionPasswordRepository.Obtener(token);
                if (recuperacion == null || recuperacion.FechaExpiracion < DateTime.Now || recuperacion.Usado == true)
                    return null;

                Usuario user = await _usuarioRepository.Obtener(recuperacion.IdUsuario);
                user.Password = _passwordService.HashPassword(user, usuario.Password);
                recuperacion.Usado = true;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return user;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Usuario> Obtener(string correo)
        {
            return await _usuarioRepository.Obtener(correo);
        }

        private async Task<bool> VerificarExistenciaUsuario(Usuario usuario)
        {
            Usuario user = await _usuarioRepository.Obtener(usuario.Correo);
            return user != null;
        }

    }
}
