using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class TasaInteresRepository
    {

        private AppDbContext _context;

        public TasaInteresRepository(AppDbContext context)
        {
            _context = context;
        }


        #region Utilidades

        public async Task<IEnumerable<TasaInteres>> Obtener()
        {
            return await _context.TASAS_INTERES.ToListAsync();
        }

        public async Task<TasaInteres> Obtener(int idTasaInteres)
        {
            return await _context.TASAS_INTERES.FirstOrDefaultAsync(t => t.IdTasaInteres == idTasaInteres);
        }

        #endregion

    }
}
