using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class BancoRepository
    {
        private readonly AppDbContext _context;

        public BancoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Banco> Agregar(Banco banco)
        {
            await _context.BANCOS.AddAsync(banco);
            if(await _context.SaveChangesAsync() > 0)
            {
                return banco;
            }
            return null;
        }

        public async Task<IEnumerable<Banco>> ObtenerTodos()
        {
            return await _context.BANCOS.ToListAsync();
        }

        public async Task<Banco?> ObtenerPorId(int id)
        {
            var banco = await _context.BANCOS
                .Include(b => b.EscenariosTasaInteres)
                    .ThenInclude(pb => pb.PlazosEscenarios)
                        .ThenInclude(ip => ip.Indicador)
                .Include(e => e.EndeudamientoMaximos)
                    .ThenInclude(t => t.TipoAsalariado)
                .Include(s => s.SeguroBancos)
                    .ThenInclude(s => s.Seguro)
                .FirstOrDefaultAsync(b => b.IdBanco == id);
            return banco;
        }

        public async Task<bool> Existe(int id)
        {
            return await _context.BANCOS.AnyAsync(b => b.IdBanco == id);
        }
    }
}
