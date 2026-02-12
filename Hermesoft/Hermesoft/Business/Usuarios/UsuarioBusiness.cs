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

        public UsuarioBusiness(AppDbContext context, PasswordService passwordService, UsuarioRepository usuarioRepository)
        {
            _context = context;
            _passwordService = passwordService;
            _usuarioRepository = usuarioRepository;
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

                usuario.Password = _passwordService.HashPassword(usuario, usuario.Password);
                await _usuarioRepository.Agregar(usuario);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return usuario;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<bool> VerificarExistenciaUsuario(Usuario usuario)
        {
            Usuario user = await _usuarioRepository.Obtener(usuario.Correo);
            return user != null;
        }
    }
}
