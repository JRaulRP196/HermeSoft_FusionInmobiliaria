using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class SeguroRepository
    {

        private AppDbContext _context;

        public SeguroRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Seguro>> Obtener()
        {
            return await _context.SEGUROS.ToListAsync();
        }
    }
}
