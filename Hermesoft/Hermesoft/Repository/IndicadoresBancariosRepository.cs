using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models.Banco;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class IndicadoresBancariosRepository
    {

        private readonly AppDbContext _context;

        public IndicadoresBancariosRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Agregar(IndicadoresBancarios indicador)
        {
            await _context.INDICADORES_BANCARIOS.AddAsync(indicador);
        }

        public async Task<IEnumerable<IndicadoresBancarios>> Obtener()
        {
            return await _context.INDICADORES_BANCARIOS.ToListAsync();
        }

    }
}
