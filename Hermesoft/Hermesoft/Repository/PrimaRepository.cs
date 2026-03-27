using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class PrimaRepository
    {
        private readonly AppDbContext _context;

        public PrimaRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Utilidades

        public async Task Agregar(Primas prima)
        {
            await _context.PRIMAS.AddAsync(prima);
        }

        public async Task<Primas> Obtener(string correoCliente)
        {
            return await _context.PRIMAS
                .Include(p => p.DesglosesPrimas)
                .FirstOrDefaultAsync(p => p.CorreoCliente == correoCliente);
        }

        public async Task<List<Primas>> ObtenerPorCorreo(string correoCliente)
        {
            return await _context.PRIMAS
                .Include(p => p.DesglosesPrimas)
                .Where(p => p.CorreoCliente == correoCliente)
                .OrderByDescending(p => p.IdPrima)
                .ToListAsync();
        }

        public async Task<Primas> ObtenerPorId(int idPrima)
        {
            return await _context.PRIMAS
                .Include(p => p.DesglosesPrimas)
                .FirstOrDefaultAsync(p => p.IdPrima == idPrima);
        }

        #endregion
    }
}