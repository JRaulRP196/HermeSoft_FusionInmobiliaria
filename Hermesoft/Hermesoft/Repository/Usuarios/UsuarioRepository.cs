using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository.Usuarios
{
    public class UsuarioRepository
    {

        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Agregar(Usuario usuario)
        {
            await _context.USUARIOS.AddAsync(usuario);
        }

        public async Task<IEnumerable<Usuario>> Obtener()
        {
            return await _context.USUARIOS
                .Include(r => r.Rol)
                .ToListAsync();
        }

        public async Task<Usuario> Obtener(string correo)
        {
            return await _context.USUARIOS
                .Include(r => r.Rol)
                .FirstOrDefaultAsync(u => u.Correo == correo);
        }

    }
}
