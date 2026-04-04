using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class MapaRepository
    {

        private readonly AppDbContext _context;

        public MapaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Mapa>> GetMapas()
        {
            return await _context.MAPAS.ToListAsync();
        }

        public async Task<Mapa> GetMapaPorCondominio(string condominio) 
        {
            return await _context.MAPAS.FirstOrDefaultAsync(m => m.condominio == condominio);
        }

        public async Task Agregar(Mapa mapa)
        {
            await _context.MAPAS.AddAsync(mapa);
        }

    }
}
