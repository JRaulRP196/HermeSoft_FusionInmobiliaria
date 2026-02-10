using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class TipoCambioRepository
    {

        private readonly AppDbContext _context;

        public TipoCambioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Agregar(TipoCambio tipoCambio)
        {
            await _context.TIPO_CAMBIO.AddAsync(tipoCambio);
        }

        public async Task<TipoCambio> Obtener()
        {
            IEnumerable<TipoCambio> tiposCambios = await _context.TIPO_CAMBIO.ToListAsync();
            return tiposCambios.FirstOrDefault();
        }

    }
}
