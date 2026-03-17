using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class VentaRepository
    {

        private readonly AppDbContext _context;

        public VentaRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Utilidades

        public async Task Agregar(Venta venta)
        {
            _context.VENTAS.Add(venta);
        }

        public async Task<List<Venta>> Obtener()
        {
            return await _context.VENTAS
                    .Include(v => v.Prima)
                        .ThenInclude(p => p.DesglosesPrimas)
                    .ToListAsync();
        }

        public async Task<Venta> Obtener(int numContrato)
        {
            return await _context.VENTAS
                    .Include(v => v.Prima)
                        .ThenInclude(p => p.DesglosesPrimas)
                    .FirstOrDefaultAsync(v => v.NumContrato == numContrato);
        }

        #endregion

    }
}
