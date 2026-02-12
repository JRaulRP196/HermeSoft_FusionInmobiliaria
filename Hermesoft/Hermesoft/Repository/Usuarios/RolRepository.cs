using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository.Usuarios
{
    public class RolRepository
    {

        private readonly AppDbContext _context;

        public RolRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rol>> Obtener()
        {
            return await _context.ROLES.ToListAsync();
        }

    }
}
