using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class SeguroBancoRepository
    {

        private AppDbContext _context;

        public SeguroBancoRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }

        #region Utilidades

        public async Task<SeguroBanco> Agregar(SeguroBanco seguro)
        {
            _context.SEGUROS_BANCOS.Add(seguro);
            if(await _context.SaveChangesAsync() > 0)
            {
                return seguro;
            }
            return null;
        }

        public async Task<IEnumerable<SeguroBanco>> ObtenerPorBanco(int idBanco)
        {

            return await _context.SEGUROS_BANCOS
                .Include(s => s.Seguro)
                .Where(s => s.IdBanco == idBanco)
                .ToListAsync();

        }

        #endregion

    }
}
