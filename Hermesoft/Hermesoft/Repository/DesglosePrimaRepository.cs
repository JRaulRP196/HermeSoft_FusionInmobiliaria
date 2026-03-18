using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class DesglosePrimaRepository
    {

        private readonly AppDbContext _context;

        public DesglosePrimaRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        public async Task<DesglosesPrimas> Obtener(int idDesglose)
        {
            return await _context.DESGLOSES_PRIMAS
                .Include(d => d.Prima)
                    .ThenInclude(p => p.Venta)
                        .ThenInclude(v => v.Usuario)
                .FirstOrDefaultAsync(d => d.IdDesglosePrima == idDesglose);
        }

    }
}
