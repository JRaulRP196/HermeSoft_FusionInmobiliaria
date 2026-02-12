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

        public UsuarioBusiness(AppDbContext context, PasswordService passwordService, UsuarioRepository usuarioRepository,
            EmailService emailService)
        {
            _context = context;
            _passwordService = passwordService;
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
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
