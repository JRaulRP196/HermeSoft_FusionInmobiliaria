using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HermeSoft_Fusion.Repository
{
    public class BancoRepository
    {
        private readonly AppDbContext _context;

        public BancoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Banco>> ObtenerTodos()
        {
            return await _context.BANCOS.ToListAsync();
        }

        public async Task<Banco?> ObtenerPorId(int id)
        {
            return await _context.BANCOS.FindAsync(id);
        }

        public async Task<bool> Existe(int id)
        {
            return await _context.BANCOS.AnyAsync(b => b.IdBanco == id);
        }
    }
}
