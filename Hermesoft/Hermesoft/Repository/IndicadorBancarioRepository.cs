using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class IndicadorBancarioRepository
    {
        private readonly AppDbContext _context;

        public IndicadorBancarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IndicadorBancario?> ObtenerPorNombre(string nombre)
        {
            return await _context.INDICADORES_BANCARIOS
                .OrderByDescending(x => x.FechaVigente)
                .FirstOrDefaultAsync(x => x.Nombre == nombre);
        }

        public async Task<List<IndicadorBancario>> ObtenerTodos()
        {
            return await _context.INDICADORES_BANCARIOS
                .OrderByDescending(x => x.FechaVigente)
                .ToListAsync();
        }

        public async Task Upsert(string nombre, decimal porcSeguro, DateTime fechaVigente)
        {
            var ind = await _context.INDICADORES_BANCARIOS
                .FirstOrDefaultAsync(x => x.Nombre == nombre);

            if (ind == null)
            {
                ind = new IndicadorBancario
                {
                    Nombre = nombre,
                    PorcSeguro = porcSeguro,
                    FechaVigente = fechaVigente
                };
                _context.INDICADORES_BANCARIOS.Add(ind);
            }
            else
            {
                ind.PorcSeguro = porcSeguro;
                ind.FechaVigente = fechaVigente;
            }

            await _context.SaveChangesAsync();
        }
    }
}
