using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class TipoAsalariadoRepository
    {

        private AppDbContext _context;

        public TipoAsalariadoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoAsalariado>> Obtener()
        {
            return await _context.TIPOS_ASALARIADOS.ToListAsync();
        }

    }
}
