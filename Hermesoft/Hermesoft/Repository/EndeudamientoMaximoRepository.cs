using HermeSoft_Fusion.Data;
using HermeSoft_Fusion.Models;
using Microsoft.EntityFrameworkCore;

namespace HermeSoft_Fusion.Repository
{
    public class EndeudamientoMaximoRepository
    {

        private AppDbContext _context;

        public EndeudamientoMaximoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EndeudamientoMaximo> Agregar(EndeudamientoMaximo endeudamiento)
        {
            await _context.ENDEUDAMIENTOS_MAXIMOS.AddAsync(endeudamiento);
            if (await _context.SaveChangesAsync() > 0)
            {
                return endeudamiento;
            }
            return null;
        }

    }
}
