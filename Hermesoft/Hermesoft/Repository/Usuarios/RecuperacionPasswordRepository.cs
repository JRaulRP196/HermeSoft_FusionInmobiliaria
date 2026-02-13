using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository.Usuarios
{
    public class RecuperacionPasswordRepository
    {

        private readonly AppDbContext _context;

        public RecuperacionPasswordRepository(AppDbContext context)
        {
            _context = context;
        }


        public async Task Agregar(RecuperacionPassword recuperacion)
        {
            await _context.RECUPERACIONES_PASSWORD.AddAsync(recuperacion);
        }

        public async Task<RecuperacionPassword> Obtener(string token)
        {
            return await _context.RECUPERACIONES_PASSWORD.FirstOrDefaultAsync(r => r.Token == token);
        }

    }
}
